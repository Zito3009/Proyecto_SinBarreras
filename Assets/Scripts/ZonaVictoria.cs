using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class ZonaVictoria : MonoBehaviour{
    // Esta variable guardará la pantalla/texto de "¡Ganaste!" que creaste en la UI
    public GameObject cartelGanaste;

    // Esta función se ejecuta automáticamente cuando algo entra en la zona verde (Is Trigger)
    private void OnTriggerEnter(Collider col)
    {
       // 1. Buscamos el script de MOVIMIENTO en la esfera que entró
        MovimientoDirecto movimiento = col.GetComponent<MovimientoDirecto>();

        // 2. Si realmente fue el jugador el que entró
        if (movimiento != null)
        {
            // Mostramos el cartel activando el objeto en el Canvas
            cartelGanaste.SetActive(true);

            // Desactivamos el script de MOVIMIENTO para que no pueda caminar
            movimiento.enabled = false;

            // Liberamos el puntero del ratón para que vuelva a ser visible en pantalla
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

