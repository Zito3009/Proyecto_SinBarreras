using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaVictoria : MonoBehaviour
{
    public GameObject cartelGanaste;

    // Elementos de la interfaz a mostrar
    public GameObject estrella;
    public GameObject estrella2;
    public GameObject cartelMejora;
    float TiempoEspera = 5f;
    bool contar;

    void Update(){
        if(contar){
            TiempoEspera -= Time.deltaTime;
            if(TiempoEspera <= 0){
                cartelMejora.SetActive(true);
                contar = false;
                cartelGanaste.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        // 1. Buscamos el script de movimiento en el objeto que entró
        MovimientoDirecto movimiento = col.GetComponent<MovimientoDirecto>();

        // 2. Si fue el jugador
        if (movimiento != null)
        {
            // Buscamos el temporizador en la escena
            TemporizadorJuego temporizador = FindObjectOfType<TemporizadorJuego>();

            if (temporizador != null)
            {
                // Detenemos la cuenta regresiva
                temporizador.enabled = false;

                // Calculamos cuánto tiempo tardó en total (90 segundos iniciales - tiempo restante)
                float tiempoEmpleado = 90f - temporizador.tiempoRestante;

                // Si tardó menos de 60 segundos (menos de 1 minuto)
                if (tiempoEmpleado < 60f)
                {
                    estrella2.SetActive(true);
                    estrella.SetActive(true);
                }
                // Si tardó 60 segundos o más (más de 1 minuto)
                else
                {
                    estrella.SetActive(true);
                    estrella2.SetActive(false);
                }
            }

            // Mostramos el panel de ganaste
            cartelGanaste.SetActive(true);

            // Desactivamos el movimiento del jugador
            movimiento.enabled = false;

            // Liberamos el ratón para poder interactuar en pantalla
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            contar = true;
        }
    }

    
}

