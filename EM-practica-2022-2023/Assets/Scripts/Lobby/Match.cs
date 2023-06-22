
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;



public class Match : MonoBehaviour
{
    //[SerializeField] private GameObject _buttons;
    private Lobby _connectedLobby;
    private QueryResponse _lobbies;
    private UnityTransport _transport;
    private const string JoinCodeKey = "j";
    private string _playerId;
    public int maxPlayers = 2;


    public static Match Instance { get; private set; }

    private async void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicialización (Unity Services)
        if (UnityServices.State != ServicesInitializationState.Initialized) // Solo se puede inicializar una vez. Si ya está inicializado, no hacemos nada
        {
            // Hacemos que los builds en un mismo ordenador tengan distintos Id
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 10000).ToString());

            // Inicialización Unity Services
            await UnityServices.InitializeAsync(initializationOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
        }
        _transport = FindObjectOfType<UnityTransport>();
    }

    public async void CreateOrJoinLobby()
    {
        //await Authenticate();

        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();

        //if (_connectedLobby != null) _buttons.SetActive(false);
    }

   /* private async Task Authenticate()
    {
        var options = new InitializationOptions();


        
        _playerId = AuthenticationService.Instance.PlayerId;
    }*/

    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            // Attempt to join a lobby in progress
            var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            // If we found one, grab the relay allocation details
            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            // Set the details to the transform
            SetTransformAsClient(a);

            // Join the game room as a client
            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"No lobbies available via quick join");
            return null;
        }
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            // Create a relay allocation and generate a join code to share with the lobby
            var a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            // Create a lobby, adding the relay join code to the lobby data
            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };
            var lobby = await Lobbies.Instance.CreateLobbyAsync("Useless Lobby Name", maxPlayers, options);

            // Send a heartbeat every 15 seconds to keep the room alive
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            // Set the game room to use the relay allocation
            _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

            // Start the room. I'm doing this immediately, but maybe you want to wait for the lobby to fill up
            NetworkManager.Singleton.StartHost();
            return lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogFormat("Failed creating a lobby");
            return null;
        }
    }

    private void SetTransformAsClient(JoinAllocation a)
    {
        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
    }

    private static IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    private void OnDestroy()
    {
        try
        {
            StopAllCoroutines();
            // todo: Add a check to see if you're host
            if (_connectedLobby != null)
            {
                if (_connectedLobby.HostId == _playerId) Lobbies.Instance.DeleteLobbyAsync(_connectedLobby.Id);
                else Lobbies.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"Error shutting down lobby: {e}");
        }
    }
} 