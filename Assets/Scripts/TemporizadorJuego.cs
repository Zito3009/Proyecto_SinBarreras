using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TemporizadorJuego : MonoBehaviour
{
    public float tiempoRestante = 90f; // 1:30 minutos
    public TextMeshProUGUI textoTiempo;
    public GameManager gameManager;
    public MovimientoDirecto movimientoJugador;


    private bool juegoTerminado = false;

    void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;

        // Muestra los segundos en pantalla
        if (textoTiempo != null){
            float tiempo = Mathf.Max(0, tiempoRestante);
            textoTiempo.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(tiempo / 60), Mathf.FloorToInt(tiempo % 60));
        }
        // Si se acaba el tiempo
        if (tiempoRestante <= 0)
        {
            juegoTerminado = true;
            movimientoJugador.enabled = false;
            gameManager.TiempoAgotado();
        }
    }
}
