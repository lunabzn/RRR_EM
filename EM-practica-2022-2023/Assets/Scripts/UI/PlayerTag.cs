using Cinemachine;
using Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTag : NetworkBehaviour
{
    public string playerName;
    [SerializeField] private GameObject AkaiKazePrefab;
    [SerializeField] private GameObject OniPrefab;
    [SerializeField] private GameObject HuntressPrefab;
    public string playerSkin;

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            string nameInput = GameObject.Find("UI").GetComponent<UIHandler>().playerName;
            SetPlayerTagServerRpc(nameInput);
            GetSettingsFromPreviousPlayerServerRpc();
        }
    }

    [ServerRpc]
    public void SetPlayerTagServerRpc(string nameInInputText)
    {
        playerName = nameInInputText;
        //SetPlayerTagClientRpc(playerName);
    }

    /*[ClientRpc]
    public void SetPlayerTagClientRpc(string playerName2)
    {
        transform.GetChild(0).Find("PlayerHUD").Find("Nombre").GetComponent<TextMeshPro>().text = playerName2;
    }*/

    [ClientRpc]
    public void SetPlayerTagClientRpc(string playerName2, int ClientID)
    {
        try
        {
            transform.GetChild(0).Find("PlayerHUD").Find("Nombre").GetComponent<TextMeshPro>().text = playerName2;
            string skinSelected = transform.GetChild(0).gameObject.name.Replace("(Clone)", "");

            var _playerHUD = GameObject.Find("HUD").transform.GetChild(ClientID);
            playerSkin = skinSelected;
            _playerHUD.gameObject.SetActive(true);
            _playerHUD.transform.Find("Nombre").GetComponent<TMPro.TextMeshProUGUI>().text = playerName2;

            //cambios de skin segun el boton pulsado
            switch (skinSelected)
            {
                case "Huntress":
                    _playerHUD.transform.Find("Personaje").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("HuntressSprite");

                    break;
                case "Oni":
                    _playerHUD.transform.Find("Personaje").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("OniSprite");

                    break;
                default://"AkaiKaze"
                    _playerHUD.transform.Find("Personaje").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("AkaiKazeSprite");
                    break;
            }
        }
        catch (Exception e) { }
    }

    [ServerRpc]
    public void GetSettingsFromPreviousPlayerServerRpc()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            string playerName = client.PlayerObject.GetComponentInChildren<PlayerTag>().playerName;
            Debug.Log(playerName);
            int playerNum = client.PlayerObject.GetComponent<PlayerNetworkConfig>().maxPlayers.Value;
            Debug.Log(playerNum);
            client.PlayerObject.GetComponentInChildren<PlayerTag>().SetPlayerTagClientRpc(playerName, playerNum);
        }
    }

 
}
