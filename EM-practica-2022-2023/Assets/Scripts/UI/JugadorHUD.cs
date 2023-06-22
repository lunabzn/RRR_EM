using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.Serialization;
using TMPro;
using System;

namespace UI
{
    public class JugadorHUD : NetworkBehaviour
    {
        public string nombreJugador;
        public string personaje;
        // Start is called before the first frame update
        void Start()
        {
            if (!IsOwner) return;
            
            string nameInInputText = GameObject.Find("UI").GetComponent<UIManager>().playerName;
            SetPlayerTagServerRpc(nameInInputText);
            GetSettingsFromPreviousPlayerServerRpc();
            Debug.Log("aaaaaaaaaaaaaaaaaaaaaa");
        }

        [ServerRpc]
        public void SetPlayerTagServerRpc(string nameInInputText)
        {
            nombreJugador = nameInInputText;
            SetPlayerTagClientRpc(nombreJugador);
        }

        [ClientRpc]
        public void SetPlayerTagClientRpc(string playerName)
        {
            transform.GetChild(0).Find("HUD").Find("PlayerTag").GetComponent<TextMeshPro>().text = nombreJugador;
        }

        [ServerRpc]
        public void GetSettingsFromPreviousPlayerServerRpc()
        {
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                string playerName = client.PlayerObject.GetComponentInChildren<JugadorHUD>().nombreJugador;
            }
        }

        [ClientRpc]
        public void ResetPlayerHUDClientRpc(string playerName, int ClientID)
        {
            try
            {
                transform.GetChild(0).Find("HUD").Find("PlayerTag").GetComponent<TextMeshPro>().text = playerName;
                var _playerHUD = GameObject.Find("HUD").transform.GetChild(ClientID);

                _playerHUD.transform.Find("PlayerName").GetComponent<TMPro.TextMeshProUGUI>().text = playerName;
                _playerHUD.gameObject.SetActive(true);
            }
            catch (Exception e)
            {

            }
        }

    }
}

