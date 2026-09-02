using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TemporizadorJuego : MonoBehaviour
{
    public float tiempoRestante = 90f; // 1:30 minutos
    public TextMeshProUGUI textoTiempo;
    public GameObject cartelPerdiste;

    private bool juegoTerminado = false;

    void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;

        // Muestra los segundos en pantalla
        if (textoTiempo != null){
    float tiempo = Mathf.Max(0, tiempoRestante);
    int minutos = Mathf.FloorToInt(tiempo / 60);
    int segundos = Mathf.FloorToInt(tiempo % 60);

    // Muestra el formato 00:00 (ej: 01:30 o 00:09)
    textoTiempo.text = string.Format("{0:00}:{1:00}", minutos, segundos);}

        // Si se acaba el tiempo
        if (tiempoRestante <= 0)
        {
            juegoTerminado = true;
            
            if (cartelPerdiste != null) 
                cartelPerdiste.SetActive(true);

            // Desactiva movimiento del jugador y libera el ratón
            FindObjectOfType<MovimientoDirecto>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
