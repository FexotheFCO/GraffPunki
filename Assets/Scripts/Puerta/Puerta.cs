using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puerta : MonoBehaviour
{
    Material material;
    Renderer renderer;
    MeshCollider meshCollider;
    

    public bool abierto;

    public Color colorAbierto;
    public Color colorCerrado;
    // Start is called before the first frame update
    void Start()
    {
        material = this.GetComponent<Material>();
        renderer = this.GetComponent<Renderer>();
        meshCollider = this.GetComponent<MeshCollider>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void abrir()
    {
        if (!abierto)
        {
            renderer.material.SetColor("_EmissionColor", colorAbierto);
            abierto = true;
            meshCollider.isTrigger = true;
        }
        else
        {
            renderer.material.SetColor("_EmissionColor", colorCerrado);
            abierto = false;
            meshCollider.isTrigger = false;
        }
    }
}
