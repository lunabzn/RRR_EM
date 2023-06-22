using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.Serialization;
using TMPro;
using UnityEngine.UI;


namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField playerNameInput;
        public string playerName = "";

        public static UIManager Instance { get; private set; }


        // Start is called before the first frame update
        void Start()
        {
            Instance= this;
 
            playerNameInput.onValueChanged.AddListener(SetName);
        }
        public void SetName(string c)
        {
            playerName = c;

        }

    }
}

