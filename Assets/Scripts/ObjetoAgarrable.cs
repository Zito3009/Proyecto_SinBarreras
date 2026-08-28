using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjetoAgarrable : MonoBehaviour
{
    private Rigidbody rb;
    private Renderer miRender;
    private Collider miCollider;
    private Color colorOriginal;

    private bool estoyAgarrado = false;
    private Transform puntoEnMano;

    [Header("Configuración")]
    public Color colorResaltado = Color.yellow;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        miRender = GetComponent<Renderer>();
        miCollider = GetComponent<Collider>();

        if (miRender != null)
        {
            colorOriginal = miRender.material.color;
        }
    }

    // Usamos FixedUpdate porque los Rigidbody se mueven en el ciclo de física
    void FixedUpdate()
    {
        if (estoyAgarrado && puntoEnMano != null)
        {
            // Forzamos al Rigidbody a moverse directamente a la posición de la mano
            rb.MovePosition(puntoEnMano.position);
            rb.MoveRotation(puntoEnMano.rotation);
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

        // Mantenemos isKinematic en true para que la física no lo atraiga al suelo
        rb.isKinematic = true;
        rb.useGravity = false;

        // Desactivamos el colisionador para evitar que choque con el personaje
        if (miCollider != null) 
        {
            miCollider.enabled = false;
        }

        // Posicionamos el objeto inmediatamente en la mano
        transform.position = mano.position;
        transform.rotation = mano.rotation;
    }

    public void Soltar()
    {
        estoyAgarrado = false;
        puntoEnMano = null;

        // Reactivamos físicas y gravedad
        rb.isKinematic = false;
        rb.useGravity = true;

        // Reactivamos colisionador
        if (miCollider != null) 
        {
            miCollider.enabled = true;
        }

        Resaltar(false);
    }
}

