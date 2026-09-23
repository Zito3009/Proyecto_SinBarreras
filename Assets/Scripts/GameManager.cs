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
    public Transform posInicio;
    public GameObject panelGanastePersonaje2;
    public GameObject panelGanasteMejorPersonaje2;
    public CamaraTerceraPersona camaraJugador;
    public ControladorAgarrar controladorAgarrarPersonaje2;
    public ContadorObjetosFase2 contadorFase2;
    public GameObject panelContador;
    public GameObject flecha;
    public Animator puerta;

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

        if (flecha != null)
        flecha.SetActive(false);

        if (puerta != null)
    {
        puerta.Play("Abrir", 0, 0f);   // fuerza el frame 0 (puerta cerrada)
        puerta.Update(0f);              // aplica esa pose ya mismo
        puerta.enabled = false;         // y la deja congelada ahí, cerrada
    }
    if (panelContador != null)
        panelContador.SetActive(true);

        // Cambiar personajes y posicionar al 2 en el inicio
        personaje1.SetActive(false);
        personaje2.transform.position = posInicio.position;
        personaje2.SetActive(true);

        camaraJugador.objetivo = personaje2.transform;
        controladorAgarrarPersonaje2.enabled = true;
        contadorFase2.IniciarFase(); 

        // Reiniciar variables
        tiempo = 0;
        termino = false;

        // Ocultar cursor y reanudar juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }
    
     public void FinalizarFase2(int objetosMovidos, int minimoRequerido)
    {
        PausarYMostrarCursor();

        if (objetosMovidos >= minimoRequerido)
            panelGanasteMejorPersonaje2.SetActive(true);
        else
            panelGanastePersonaje2.SetActive(true);
    }

    void PausarYMostrarCursor()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
}
}
