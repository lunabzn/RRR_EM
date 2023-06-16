using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vida : MonoBehaviour
{
    public float vidaMax = 1500.0f;
    [Range(0.0f, 1500.0f)]
    public float vidaNueva;

    public float getVidaMax()
    {
        return vidaMax;
    }
    public float getVidaNueva()
    {
        return vidaNueva;
    }
    public void setVidaNueva(float vN)
    {
        this.vidaNueva = vN;
    }
    // Start is called before the first frame update
    void Start()
    {
        vidaNueva = vidaMax;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
