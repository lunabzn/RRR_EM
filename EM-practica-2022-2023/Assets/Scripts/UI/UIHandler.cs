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
    public class UIHandler : MonoBehaviour
    {
        //Coger nombre del input
        [SerializeField] private TMP_InputField playerNameInput;
        public string playerName = "";

        //elegir personaje
        [SerializeField] public GameObject characterSelectionPanel;
        [SerializeField] public Button initiatonButton;
        [SerializeField] private Button akaiKazeButton;
        [SerializeField] private Button oniButton;
        [SerializeField] private Button huntressButton;
        public string playerSkin;

        public static UIHandler Instance { get; private set; }

        void Start()
        {
            Instance = this;
            playerNameInput.onValueChanged.AddListener(seleccionarNombre);

            akaiKazeButton.onClick.AddListener(() => seleccionarPersonaje("Akai Kaze"));
            oniButton.onClick.AddListener(() => seleccionarPersonaje("Oni"));
            huntressButton.onClick.AddListener(() => seleccionarPersonaje("Huntress"));

            initiatonButton.onClick.AddListener(PlayerInitiation);
        }

        private void seleccionarNombre(string nombre)
        {
            playerName = nombre;
            //si tenemos nombre y sprite seleccionado visibilizamos boto inicio
            if (playerName != "" && playerSkin != "") { initiatonButton.gameObject.SetActive(true); }
        }

        private void seleccionarPersonaje(string personaje)
        {
            playerSkin = personaje;
            if (playerName != "" && playerSkin != "") { initiatonButton.gameObject.SetActive(true); }
        }

        private void PlayerInitiation()
        {
            characterSelectionPanel.SetActive(false);

        }

    }
}