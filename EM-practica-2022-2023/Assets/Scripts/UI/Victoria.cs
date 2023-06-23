using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Movement.Components;
using Unity.VisualScripting;
using System.Threading;
using TMPro;

public class Victoria : NetworkBehaviour
{

    public NetworkVariable<int> alivePlayersRemaining = new NetworkVariable<int>(); //Jugadores que quedan vivos (Protegida)


    //public NetworkVariable<bool> temporizadorEnMarcha = new NetworkVariable<bool>(false); //Variable para cuando se acabe el temporizador salte la pantalla de victoria correspondiente

    public int playersInGame = 0; //Variables para almacenar los jugadores que se han conectado


    [SerializeField] GameObject victoryPanel; //Referencia hacia el panel de victoria
    //[SerializeField] GameObject timerPanel;
    //[SerializeField] TextMeshProUGUI winningText;

    bool once = true;
    bool doOnce = true;


    //Comenzamos haciendo que el server o host de la partida calcule cuantos jugadores hay en función de las conexiones o desconexiones
    public override void OnNetworkSpawn()
    {
        if (IsHost || IsServer)  //Solo el servidor (o el host) es quien lleva la cuenta de los jugadores presentes
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += addPlayer; //Añadimos un jugador cuando se haya conectado
            NetworkManager.Singleton.OnClientDisconnectCallback += removePlayer; //En caso de desconexión del cliente, quitamos un jugador

            //temporizadorEnMarcha.Value = false;
            //temporizadorEnMarcha.OnValueChanged += CheckTemporizador;

            if (IsServer)
            {
                playersInGame--; //Restamos el servidor como jugador 
            }

        }
    }

    //Función en la que añadimos el número de personajes para pdoer trabajar con ello
    private void addPlayer(ulong id, string sceneName, LoadSceneMode mode)
    {
        playersInGame += 1;
        //TODO VER CUANDO ESTEN TODOS
        if (playersInGame == NetworkManager.Singleton.ConnectedClients.Count) //En cuanto todos los personajes estén (AQUI PASAMOS EL NUMERO DE JUGADORES QUE HAY EN EL JUEGO)
        {
            //timerPanel.GetComponent<Timer>().enMarcha = true;
            //ActivateTimePanelClientRpc(); //Activamos el temporizador

            alivePlayersRemaining.Value = playersInGame; //Asignamos el numero de jugador cogidos a la variable alive players
            alivePlayersRemaining.OnValueChanged += CheckNumberOfAlivePlayers; //Comprueba el numero de jugadores vivos (Para luego lanzar la condición de victoria)

            /*var fighterMovementOfPlayer = FindObjectsOfType<FighterMovement>(); //Buscamos el script de todos los personajes que se encarga de manejar el movimiento
            foreach (FighterMovement fighterMovement in fighterMovementOfPlayer) //Lo activamos ya que por defecto se encuentra desactivado para evitar que se puedan mover antes de que se hayan conectado todos los jugadores
            {
                fighterMovement.speed = 3;
                fighterMovement.jumpAmount = 1.2f;
            }*/
        }
    }

    //Método para quitar personajes (a la variable)
    private void removePlayer(ulong id)
    {
        playersInGame -= 1;
        FighterMovement[] players = FindObjectsOfType<FighterMovement>();
        Debug.Log("Desconectado: " + id);
        foreach (FighterMovement f in players)
        {
            Debug.Log("Mirando: " + f.OwnerClientId);
            if (f.OwnerClientId == id)
            {
                Debug.Log("Pongo la vida de alguien a 0");
                Destroy(f);
            }
        }

        if (playersInGame == 1) //En caso de quedar solo un juagador (Para cuando se descontecten los jugadores)
        {
            ActivateEndGameCanvasClientRpc();
        }
    }

    //En caso de desconexión actualizamos la variables correspondiente llamando a los métodos necesarios
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= addPlayer;
            NetworkManager.Singleton.OnClientDisconnectCallback -= removePlayer;

            alivePlayersRemaining.OnValueChanged -= CheckNumberOfAlivePlayers;
        }

    }

    //Aquí comprobamos cuantos jugadores quedan vivo/en partida, en caso de quedar uno se activa la condición de victoria
    public void CheckNumberOfAlivePlayers(int oldValue, int newValue)
    {
        Debug.Log(newValue);
        if (newValue == 1) //AQUÍ VAMOS PASANDO EL PARANMETRO DE COMPROBAR CUANTOS QUEDAN VIVO
        {
            //timerPanel.GetComponent<Timer>().enMarcha = false;
            ActivateEndGameCanvasClientRpc(); //Activamos el canvas de victoria
        }
    }


    /*void CheckTemporizador(bool oldValue, bool newValue)
    {
        if (newValue == false) //AQUÍ VAMOS COMPROBANDO SI EL TEMPORIZADOS ESTÁ PARADO (YA HA TERMINADO)
        {

            ActivateEndGameCanvasClientRpc();
            var fighterMovementOfPlayer = FindObjectsOfType<FighterMovement>(); //Buscamos el script de todos los personajes que se encarga de manejar el movimiento
            foreach (FighterMovement fighterMovement in fighterMovementOfPlayer) //Lo activamos ya que por defecto se encuentra desactivado para evitar que se puedan mover antes de que se hayan conectado todos los jugadores
            {
                fighterMovement.speed = 0;
                fighterMovement.jumpAmount = 0f;
            }

        }
    }*/


    [ServerRpc(RequireOwnership = false)]
    public void playerDisconnectedServerRpc()
    {
        if (alivePlayersRemaining != null)
        {
            alivePlayersRemaining.Value--;
        }
    }


    /*[ClientRpc]
    void ActivateTimePanelClientRpc() //Para el panel del tiempo
    {
        timerPanel.GetComponent<CanvasGroup>().alpha = 1;

    }*/

    [ClientRpc]
    void ActivateEndGameCanvasClientRpc() //Para el canvas de victoria
    {
        /*if (!doOnce) return;
        doOnce = false;
        SpawningBehaviour[] spawninBehaviourArray = FindObjectsOfType<SpawningBehaviour>();
        SpawningBehaviour lastPlayer = spawninBehaviourArray[0];

        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        Debug.Log("Al sacar el cartel quedan " + spawninBehaviourArray.Length);
        Debug.Log("Al sacar el cartel quedan PH " + players.Length);
        int currentBestHealth;
        foreach (var x in spawninBehaviourArray)
        {

            if (x != null)
            {
                if (x.transform.childCount > 0)
                {

                    if (lastPlayer.GetComponentInChildren<PlayerHealth>() == null)
                    {
                        currentBestHealth = 0;
                    }
                    else
                    {
                        currentBestHealth = lastPlayer.GetComponentInChildren<PlayerHealth>().Health.Value;
                    }
                    Debug.Log(x.playerId.Value);
                    Debug.Log("Vida actual: " + x.GetComponentInChildren<PlayerHealth>().Health.Value);
                    Debug.Log("CurrentBestHealth: " + currentBestHealth);
                    Debug.Log("LastPlayer: " + lastPlayer.playerId.Value);
                    Debug.Log("Esta vivo: " + x.GetComponentInChildren<PlayerHealth>().isAlive);
                    if (currentBestHealth + 1 <= x.GetComponentInChildren<PlayerHealth>().Health.Value && x.GetComponentInChildren<PlayerHealth>().isAlive)
                    {
                        Debug.Log("Decido cambiar");
                        lastPlayer = x;
                    }
                }
                else { currentBestHealth = 0; }
            }
        }
        Debug.Log("LastPlayer: " + lastPlayer.playerId.Value);
        winningText.text = lastPlayer.playerName.Value.ToString() + " GANA!";
        victoryPanel.SetActive(true);
        timerPanel.GetComponent<CanvasGroup>().alpha = 0f; //Desactivamos la ceunta atrás ya que no nos interesa*/
        victoryPanel.SetActive(true);
        esperarParaCambiar(4);
    }


    //Para volver a jugar una vez se haya terminado la partida anterior
    [ServerRpc(RequireOwnership = false)]
    public void RematchServerRpc()
    {
        DontDestroyOnLoad(gameObject);
        if (once)
        {
            once = false;
            Debug.Log("Antes de rutina");

            SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.LoadScene("Inicio", LoadSceneMode.Single);
        }
        Destroy(gameObject);
    }

    IEnumerator esperarParaCambiar(int i)
    {
        Debug.Log("Dentro de rutina");
        yield return new WaitForSeconds(i);
        RematchServerRpc();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            //temporizadorEnMarcha.Value = timerPanel.GetComponent<Timer>().enMarcha;
        }
    }
}