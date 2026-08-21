using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class ZonaVictoria : MonoBehaviour
{
    [Header("Interfaz de Usuario")]
    [Tooltip("Arrastra aquí el objeto del texto de ganaste que creaste en el Canvas")]
    public GameObject cartelGanaste;

    [Header("Efectos Adicionales (Opcional)")]
    public bool pausarJuegoAlGanar = true;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entró a la zona es el personaje
        if (other.CompareTag("Player") || other.gameObject.GetComponent<ControladorAgarrar>() != null)
        {
            MostrarVictoria();
        }
    }

    void MostrarVictoria()
    {
        if (cartelGanaste != null)
        {
            cartelGanaste.SetActive(true); // Activa el cartel en pantalla
        }

        Debug.Log("¡El jugador ha llegado a la esquina y ganó!");

        if (pausarJuegoAlGanar)
        {
            // Pausa el movimiento y las físicas del juego
            Time.timeScale = 0f;
            
            // Libera el cursor del ratón para que el jugador pueda usar menús si los hubiera
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
