using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class BarraDeVida : MonoBehaviour
{

    [SerializeField] private float vidaMaxima;
    [SerializeField] GameObject relleno;
    [SerializeField] GameObject nombre;

    //Actualizar relleno
    public void CambiarBarra(int current)
    {

        GetComponent<Image>().fillAmount = current / vidaMaxima;

        relleno.GetComponent<Image>().fillAmount = current / vidaMaxima;
    }

    //Nombre en las barras
    public void SetNombre(String n)
    {
        nombre.GetComponent<TMP_Text>().text = n;
    }
}
