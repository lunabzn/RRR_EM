using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CrearPartida : MonoBehaviour
{

    public string nombre;


    public void nombreJugador(string nombre)
    {
        this.nombre = nombre;
    }

    public void tamanioSala(int indice)
    {
        switch (indice)
        {
            case 0:
                Match.Instance.maxPlayers = 2; 
                break;
            case 1:
                Match.Instance.maxPlayers = 3;
                break;
            case 2:
                Match.Instance.maxPlayers = 4;
                break;
        }
    }

    public void tiempoJuego(int indice)
    {
        switch (indice)
        {
            case 0:
                //Cronometro.Instance.tiempo = 30; Luego en la clase cronometro haces la misma estructura que en la clase match creando la instancia
                break;
            case 1:
                //Cronometro.Instance.tiempo = 60;
                break;
            case 2:
                //Cronometro.Instance.tiempo = 90;
                break;
        }
    }

}
