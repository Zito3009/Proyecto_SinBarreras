using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjetoAgarrable : MonoBehaviour
{
    private Rigidbody rb;

    // Cambiamos a arreglos [] para guardar todas las partes del objeto
    private Collider[] misColliders;
    private Renderer[] misRenderers;
    private List<Color> coloresOriginales = new List<Color>();

    private bool estoyAgarrado = false;
    private Transform puntoEnMano;

    [Header("Configuración")]
    public Color colorResaltado = Color.yellow;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Obtenemos todos los Colliders y Renderers del objeto y de sus hijos (cuerpo, cierres, tiras, etc.)
        misColliders = GetComponentsInChildren<Collider>();
        misRenderers = GetComponentsInChildren<Renderer>();

        // Guardamos el color original de cada parte por separado
        coloresOriginales.Clear();
        foreach (Renderer r in misRenderers)
        {
            if (r != null && r.material != null)
            {
                coloresOriginales.Add(r.material.color);
            }
        }
    }

    void FixedUpdate()
    {
        if (estoyAgarrado && puntoEnMano != null)
        {
            // Movemos el Rigidbody hacia la posición y rotación de la mano
            rb.MovePosition(puntoEnMano.position);
            rb.MoveRotation(puntoEnMano.rotation);
        }
    }

    public void Resaltar(bool activar)
    {
        if (misRenderers == null || misRenderers.Length == 0) return;

        // Recorremos cada parte y le asignamos su color (resaltado u original)
        for (int i = 0; i < misRenderers.Length; i++)
        {
            if (misRenderers[i] != null && i < coloresOriginales.Count)
            {
                misRenderers[i].material.color = activar ? colorResaltado : coloresOriginales[i];
            }
        }
    }

    public void Agarrar(Transform mano)
    {
        estoyAgarrado = true;
        puntoEnMano = mano;

        rb.isKinematic = true;
        rb.useGravity = false;

        // Desactivamos todos los colisionadores para que no choque con el jugador
        if (misColliders != null)
        {
            foreach (Collider col in misColliders)
            {
                if (col != null) col.enabled = false;
            }
        }

        transform.position = mano.position;
        transform.rotation = mano.rotation;
    }

    public void Soltar()
    {
        estoyAgarrado = false;
        puntoEnMano = null;

        rb.isKinematic = false;
        rb.useGravity = true;

        // Reactivamos todos los colisionadores al soltar
        if (misColliders != null)
        {
            foreach (Collider col in misColliders)
            {
                if (col != null) col.enabled = true;
            }
        }

        Resaltar(false);
    }

    private bool yaDepositado = false;

    public bool PuedeContarseComoDepositado() => !estoyAgarrado && !yaDepositado;
    public void MarcarComoDepositado() => yaDepositado = true;

    public void Guardar()
    {
        Resaltar(false);
        gameObject.SetActive(false);
    }

    public void Depositar()
    {
        gameObject.SetActive(false);
    }
}