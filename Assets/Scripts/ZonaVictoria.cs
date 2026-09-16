using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaVictoria : MonoBehaviour
{
    public GameManager gameManager; // Referencia a tu GameManager
    public Animator animatorPuerta;
    public float duracionAnimacionPuerta = 2.1f; // ajustá este número a la duración real de tu clip

    private bool yaActivada = false;

    private void OnTriggerEnter(Collider col)
    {
        if (yaActivada) return;

        MovimientoDirecto movimiento = col.GetComponentInParent<MovimientoDirecto>();
        if (movimiento == null) return;

        yaActivada = true;
        movimiento.enabled = false;

        if (animatorPuerta != null)
            animatorPuerta.Play("Abrir");

        StartCoroutine(EsperarYMostrarCartel());
    }

    private IEnumerator EsperarYMostrarCartel()
    {
        yield return new WaitForSeconds(duracionAnimacionPuerta);
        gameManager.TocarPuerta();
    }
}

