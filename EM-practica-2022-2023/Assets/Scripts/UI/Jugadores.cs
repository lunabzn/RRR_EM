using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jugadores : MonoBehaviour
{
    public int jugadores = 0;
    public int actualJugadores;

    public int getJugadores()
    {
        return jugadores;
    }
    public int getActualJugadores()
    {
        return actualJugadores;
    }
    public void setActualJugadores(int jug)
    {
        this.actualJugadores = jug;
    }
    // Start is called before the first frame update
    void Start()
    {
        actualJugadores = jugadores;
    }
}
