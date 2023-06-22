using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIHandler : MonoBehaviour
    {
        public GameObject debugPanel;
        public Button hostButton;
        public Button clientButton;
        public Jugadores jugadores;
        [SerializeField] public NetworkVariable<int> numJugadores = new NetworkVariable<int>();


        void Start()
        {
            jugadores = GameObject.FindObjectOfType<Jugadores>();
            inicializarJugadoresServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void inicializarJugadoresServerRpc()
        {
            numJugadores.Value = jugadores.getJugadores();
        }


        public void OnHostButtonClicked()
        {
            int j = jugadores.getActualJugadores();
            jugadores.setActualJugadores(j +1);
            NetworkManager.Singleton.StartHost();
        }


        public void OnClientButtonClicked()
        {
            if(jugadores.getActualJugadores()<=2)
            {
                int j = jugadores.getActualJugadores();
                jugadores.setActualJugadores(j + 1);
                NetworkManager.Singleton.StartClient();
               
            }     
        }
    }
}