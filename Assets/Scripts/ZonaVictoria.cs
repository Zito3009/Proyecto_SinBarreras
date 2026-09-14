using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaVictoria : MonoBehaviour
{
    public GameManager gameManager; // Referencia a tu GameManager

    private void OnTriggerEnter(Collider col)
    {
        // Verifica si el objeto que entró tiene el script de movimiento
        MovimientoDirecto movimiento = col.GetComponent<MovimientoDirecto>();

        if (movimiento != null)
        {
            // Desactiva el movimiento del jugador inmediatamente
            movimiento.enabled = false;

            // Le avisa al GameManager que tocaste la victoria para que active paneles y la cuenta
            gameManager.TocarPuerta();
        }
    }
}

