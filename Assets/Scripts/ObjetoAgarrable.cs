using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjetoAgarrable : MonoBehaviour
{
    private Rigidbody rb;
    private Renderer miRender;
    private Color colorOriginal;

    [Header("Configuracion de Color")]
    public Color colorResaltado = Color.yellow; // Color cuando te acercas

    private bool estoyAgarrado = false;
    private Transform puntoEnMano;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        miRender = GetComponent<Renderer>();

        // Guardamos el color original del objeto para volver a ponerlo despues
        if (miRender != null)
        {
            colorOriginal = miRender.material.color;
        }
    }

    void Update()
    {
        // Si estoy agarrado, me muevo a la posicion de la mano del jugador
        if (estoyAgarrado && puntoEnMano != null)
        {
            transform.position = puntoEnMano.position;
        }
    }

    // Funciones para encender y apagar el resplandor/resaltado
    public void Resaltar(bool activar)
    {
        if (miRender == null) return;

        if (activar)
        {
            miRender.material.color = colorResaltado;
        }
        else
        {
            miRender.material.color = colorOriginal;
        }
    }

    // Se ejecuta cuando la mano del jugador lo agarra
    public void Agarrar(Transform mano)
    {
        estoyAgarrado = true;
        puntoEnMano = mano;
        rb.isKinematic = true; // Desactivamos la fisica para que no se caiga mientras lo llevamos
    }

    // Se ejecuta cuando lo soltamos
    public void Soltar()
    {
        estoyAgarrado = false;
        puntoEnMano = null;
        rb.isKinematic = false; // Reactivamos la fisica para que caiga al suelo por gravedad
        Resaltar(false);
    }
}
