using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject panelPerdiste;
    public GameObject panelGanaste;
    public GameObject panelGanasteMejor;
    public GameObject panelMejora;
    public GameObject personaje1, personaje2;
    public Transform posicionInicio;

    private float tiempo;
    private bool termino;

    void Update()
    {
        if (!termino && Time.timeScale > 0)
            tiempo += Time.deltaTime;
    }

    // Llama a esto si tu temporizador de pantalla llega a cero
    public void TiempoAgotado()
    {
        termino = true;
        PausarYMostrarCursor();
        panelPerdiste.SetActive(true);
    }

    // Se ejecuta al tocar la puerta
    public void TocarPuerta()
    {
        termino = true;
        PausarYMostrarCursor();

        if (tiempo < 40f)
            panelGanasteMejor.SetActive(true);
        else
            panelGanaste.SetActive(true);

        StartCoroutine(EsperarMejora());
    }

    IEnumerator EsperarMejora()
    {
        yield return new WaitForSecondsRealtime(10f); // Espera 10 segundos
        panelGanaste.SetActive(false);
        panelGanasteMejor.SetActive(false);
        panelMejora.SetActive(true);
    }

    // Asignar al botón "Comenzar" del panelMejora
    public void EmpezarSegundaPerspectiva()
    {
        panelMejora.SetActive(false);
        
        // Cambiar personajes y posicionar al 2 en el inicio
        personaje1.SetActive(false);
        personaje2.transform.position = posInicio.position;
        personaje2.SetActive(true);

        // Reiniciar variables
        tiempo = 0;
        termino = false;

        // Ocultar cursor y reanudar juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }

    void PausarYMostrarCursor()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
}
}
