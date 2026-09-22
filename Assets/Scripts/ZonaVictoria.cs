using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaVictoria : MonoBehaviour
{
    public GameManager gameManager;
    public Animator animatorPuerta;
    public float duracionAnimacionPuerta = 2.5f;

    private bool yaActivada = false;

    private void OnTriggerEnter(Collider col)
    {
        if (yaActivada) return;

        MovimientoDirecto movimiento = col.GetComponentInParent<MovimientoDirecto>();
        if (movimiento == null) return;

        yaActivada = true;
        movimiento.enabled = false;

        if (animatorPuerta != null)
        {
            animatorPuerta.enabled = true;
            animatorPuerta.Play("Abrir", 0, 0f);
        }

        StartCoroutine(EsperarYMostrarCartel());
    }

    private IEnumerator EsperarYMostrarCartel()
    {
        yield return new WaitForSeconds(duracionAnimacionPuerta);
        gameManager.TocarPuerta();
    }
}

