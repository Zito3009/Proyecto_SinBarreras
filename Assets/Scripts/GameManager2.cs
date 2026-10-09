using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager2 : MonoBehaviour
{
    public enum Estado { Instrucciones, ElegirTiro, Corriendo, Tirando, Fin }

    public Estado estado;
    public bool esTriple;
    public int lineasActivas;

    [Header("Reglas")]
    public int puntosObjetivo = 8;
    public int intentosMaximos = 5;
    public int lineasDoble = 6;
    public int lineasTriple = 5;
    public float tiempoPorIntento = 60f;
    public string escenaSiguiente = ""; 

    [Header("Jugador")]
    public CorredorJugador corredor;

    [Header("Paneles")]
    public GameObject panelInstrucciones;
    public GameObject panelElegirTiro;
    public GameObject panelGanaste;
    public GameObject panelGanasteMejor;
    public GameObject panelGanasteCinco;
    public GameObject panelPerdiste;
    public GameObject panelPuntaje;
    public GameObject panelTiempo;

    [Header("Botón 'Seguir jugando' de cada cartel que lo tiene")]
    public GameObject botonSeguirGanaste;
    public GameObject botonSeguirGanasteMejor;

    [Header("HUD")]
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoTiempo;

    [Header("Cancha")]
    public GameObject[] lineas;

    private int puntos;
    private int intentosUsados;
    private bool huboFallo;
    private bool ganoMejor;
    private bool modoExtra;
    private float tiempoRestante;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (panelElegirTiro != null) panelElegirTiro.SetActive(false);
        if (panelGanaste != null) panelGanaste.SetActive(false);
        if (panelGanasteMejor != null) panelGanasteMejor.SetActive(false);
        if (panelGanasteCinco != null) panelGanasteCinco.SetActive(false);
        if (panelPerdiste != null) panelPerdiste.SetActive(false);
        if (panelInstrucciones != null) panelInstrucciones.SetActive(true);
        MostrarHUD(false);
        panelInstrucciones.SetActive(true);
        estado = Estado.Instrucciones;

        AjustarEstadoCursor(false);
        if (corredor != null) corredor.Detener();

        ActivarLineas(0);
        tiempoRestante = tiempoPorIntento;
        ActualizarTextoTiempo();
        ActualizarHUD();
    }

    void Update()
   {
        if (estado != Estado.Corriendo && estado != Estado.Tirando)
        {
            AjustarEstadoCursor(false);
        }

        if (estado == Estado.Corriendo)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarTextoTiempo();

            if (tiempoRestante <= 0f)
            {
                tiempoRestante = 0f;
                TerminarIntento(false); // Se acabó el tiempo
            }
        }
        else if (estado == Estado.Tirando)
        {
            // TEMPORAL: Teclas de prueba para tiro
            if (Input.GetKeyDown(KeyCode.C)) TerminarIntento(true);
            if (Input.GetKeyDown(KeyCode.F)) TerminarIntento(false);
        }
    }

    void AjustarEstadoCursor(bool bloqueadoEnJuego)
    {
        Cursor.lockState = bloqueadoEnJuego ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !bloqueadoEnJuego;
    }

    public void AceptarInstrucciones()
    {
        panelInstrucciones.SetActive(false);
        MostrarElegirTiro();
    }

    void MostrarElegirTiro()
    {
        estado = Estado.ElegirTiro;
        panelElegirTiro.SetActive(true);
        AjustarEstadoCursor(false);
    }

    public void ElegirDoble() { EmpezarIntento(false); }
    public void ElegirTriple() { EmpezarIntento(true); }

    void EmpezarIntento(bool triple)
    {
        esTriple = triple;
        panelElegirTiro.SetActive(false);
        lineasActivas = triple ? lineasTriple : lineasDoble;
        ActivarLineas(triple ? lineasTriple : lineasDoble);
        tiempoRestante = tiempoPorIntento;
        ActualizarTextoTiempo();
        estado = Estado.Corriendo;
        MostrarHUD(true);
        AjustarEstadoCursor(true);
        if (corredor != null) corredor.Reiniciar();
        estado = Estado.Corriendo;
    }

    void ActivarLineas(int cantidad)
    {
        for (int i = 0; i < lineas.Length; i++){
            if (lineas[i] != null)
                lineas[i].SetActive(i < cantidad);}
    }

    public void LlegoAlFinal()
    {
        estado = Estado.Tirando;
    }

    public void TerminarIntento(bool convertido)
    {
        intentosUsados++;

        if (convertido)
            puntos += esTriple ? 3 : 2;
        else
            huboFallo = true;

        ActualizarHUD();
        ActivarLineas(0);
        MostrarHUD(false);
        if (corredor != null) corredor.Detener();
        AjustarEstadoCursor(false);

        if (modoExtra)
        {
            if (intentosUsados >= intentosMaximos)
                MostrarVictoria(false);
            else
                MostrarElegirTiro();
            return;
        }

        if (puntos >= puntosObjetivo)
        {
            ganoMejor = !huboFallo; 

            if (intentosUsados >= intentosMaximos)
                MostrarVictoriaCinco();
            else
                MostrarVictoria(true);   
        }
        else if (puntos + (intentosMaximos - intentosUsados) * 3 < puntosObjetivo)
        {
            estado = Estado.Fin;
            panelPerdiste.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            MostrarElegirTiro();
        }
    }

    void MostrarVictoria(bool conBotonSeguir)
    {
        GameObject panel = ganoMejor ? panelGanasteMejor : panelGanaste;
        GameObject botonSeguir = ganoMejor ? botonSeguirGanasteMejor : botonSeguirGanaste;

        if (botonSeguir != null)
            botonSeguir.SetActive(conBotonSeguir);

        estado = Estado.Fin;
        panel.SetActive(true);
        AjustarEstadoCursor(false);
        Time.timeScale = 0f;
    }

    void MostrarVictoriaCinco()
    {
        estado = Estado.Fin;
        panelGanasteCinco.SetActive(true);
        AjustarEstadoCursor(false);
        Time.timeScale = 0f;
    }

    public void SeguirJugando()
    {
        panelGanaste.SetActive(false);
        panelGanasteMejor.SetActive(false);
        modoExtra = true;
        Time.timeScale = 1f;
        MostrarElegirTiro();
    }

    public void SiguienteNivel()
    {
        if (string.IsNullOrEmpty(escenaSiguiente)) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaSiguiente);
    }

    void ActualizarHUD()
    {
        if (textoPuntos != null)
            textoPuntos.text = puntos + " / " + puntosObjetivo;
    }

    void MostrarHUD(bool mostrar)
    {
        if (panelTiempo != null) panelTiempo.SetActive(mostrar);
        if (panelPuntaje != null) panelPuntaje.SetActive(mostrar);
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ActualizarTextoTiempo()
    {
        if (textoTiempo == null) return;
        int total = Mathf.CeilToInt(tiempoRestante);
        textoTiempo.text = string.Format("{0:00}:{1:00}", total / 60, total % 60);
    }
}