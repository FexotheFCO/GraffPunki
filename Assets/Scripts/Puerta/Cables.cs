using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cables : MonoBehaviour
{
    // Start is called before the first frame updateMaterial material;
    Renderer renderer;


    public bool abierto;

    public Color colorAbierto;
    public Color colorCerrado;
    public List<GameObject> cables;
    void Start()
    {
        renderer = this.GetComponent<Renderer>();
    }

    public void abrir()
    {
        foreach (GameObject a in cables)
        {
            renderer = a.GetComponent<Renderer>();
            if (!abierto)
            {
                renderer.material.SetColor("_EmissionColor", colorAbierto * 3.0f);
                
            }
            else
            {
                renderer.material.SetColor("_EmissionColor", colorCerrado * 3.0f);
               
            }
        }
        if (!abierto)
        {
            abierto = true;
        }
        else
        {
            abierto = false;
        }
        
    }
}
