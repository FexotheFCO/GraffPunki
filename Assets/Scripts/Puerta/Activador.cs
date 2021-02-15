using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activador : MonoBehaviour
{
    // Start is called before the first frame update
    public Light light;
    public Puerta puerta;
    public Cables cables;

    Material material;
    Renderer renderer;

    public bool abierto;

    public Color colorAbierto;
    public Color colorCerrado;
    void Start()
    {
        material = this.GetComponent<Material>();
        renderer = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activar()
    {
        puerta.abrir();
        cables.abrir();

        if (!abierto)
        {
            light.color = colorAbierto;
            renderer.material.SetColor("_EmissionColor", colorAbierto * 3.0f);
            abierto = true;
        }
        else
        {
            light.color = colorCerrado;
            renderer.material.SetColor("_EmissionColor", colorCerrado * 3.0f);
            abierto = false;
        }
    }
}
