using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjetoAgarrable : MonoBehaviour
{
    private Rigidbody rb;
    private Renderer miRender;
    private Collider miCollider; // Guardamos la referencia al colisionador
    private Color colorOriginal;

    private bool estoyAgarrado = false;
    private Transform puntoEnMano;

    [Header("Configuracion de Color")]
    public Color colorResaltado = Color.yellow;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        miRender = GetComponent<Renderer>();
        miCollider = GetComponent<Collider>(); // Obtenemos el Collider del objeto

        if (miRender != null)
        {
            colorOriginal = miRender.material.color;
        }
    }

    void Update()
    {
        if (estoyAgarrado && puntoEnMano != null)
        {
            transform.position = puntoEnMano.position;
            transform.rotation = puntoEnMano.rotation; // Copiamos la rotacion de la mano
        }
    }

    public void Resaltar(bool activar)
    {
        if (miRender == null) return;
        miRender.material.color = activar ? colorResaltado : colorOriginal;
    }

    public void Agarrar(Transform mano)
    {
        estoyAgarrado = true;
        puntoEnMano = mano;
        rb.isKinematic = true;

        // Desactivamos el colisionador mientras lo llevamos para que no empuje al jugador
        if (miCollider != null)
        {
            miCollider.enabled = false;
        }
    }

    public void Soltar()
    {
        estoyAgarrado = false;
        puntoEnMano = null;
        rb.isKinematic = false;

        // Volvemos a activar el colisionador al soltarlo para que vuelva a tener fisica y colisione con el piso
        if (miCollider != null)
        {
            miCollider.enabled = true;
        }

        Resaltar(false);
    }
}
