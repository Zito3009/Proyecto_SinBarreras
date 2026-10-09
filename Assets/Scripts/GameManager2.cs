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

    [Header("Reglas")]
    public int puntosObjetivo = 8;
    public int intentosMaximos = 5;
    public int lineasDoble = 6;
    public int lineasTriple = 5;
    public string escenaSiguiente = ""; // nombre de la escena del siguiente nivel (cuando exista)

    [Header("Paneles")]
    public GameObject panelInstrucciones;
    public GameObject panelElegirTiro;
    public GameObject panelGanaste;
    public GameObject panelGanasteMejor;
    public GameObject panelGanasteCinco;
    public GameObject panelPerdiste;
    public GameObject panelPuntaje;

    [Header("Botón 'Seguir jugando' de cada cartel que lo tiene")]
    public GameObject botonSeguirGanaste;
    public GameObject botonSeguirGanasteMejor;

    [Header("HUD")]
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoIntentos;

    [Header("Cancha")]
    public GameObject[] lineas; // array que se como usarlo

    private int puntos;
    private int intentosUsados;
    private bool huboFallo;
    private bool ganoMejor;
    private bool modoExtra;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        panelElegirTiro.SetActive(false);
        panelGanaste.SetActive(false);
        panelGanasteMejor.SetActive(false);
        panelGanasteCinco.SetActive(false);
        panelPerdiste.SetActive(false);
        panelInstrucciones.SetActive(true);
        estado = Estado.Instrucciones;

        ActivarLineas(0);
        ActualizarHUD();
    }

    void Update()
    {
        // TEMPORAL para probar las reglas: C = tiro convertido, F = tiro fallado. Borrar más adelante.
        if (estado == Estado.Corriendo)
        {
            if (Input.GetKeyDown(KeyCode.C)) TerminarIntento(true);
            if (Input.GetKeyDown(KeyCode.F)) TerminarIntento(false);
        }
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
    }

    public void ElegirDoble() { EmpezarIntento(false); }
    public void ElegirTriple() { EmpezarIntento(true); }

    void EmpezarIntento(bool triple)
    {
        esTriple = triple;
        panelElegirTiro.SetActive(false);
        ActivarLineas(triple ? lineasTriple : lineasDoble);
        estado = Estado.Corriendo;
    }

    void ActivarLineas(int cantidad)
    {
        for (int i = 0; i < lineas.Length; i++)
            lineas[i].SetActive(i < cantidad);
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
        Time.timeScale = 0f;
    }

    void MostrarVictoriaCinco()
    {
        estado = Estado.Fin;
        panelGanasteCinco.SetActive(true);
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
            textoPuntos.text = puntos >= puntosObjetivo ? "Puntos: " + puntos : "Puntos: " + puntos + " / " + puntosObjetivo;
        if (textoIntentos != null)
            textoIntentos.text = "Intento: " + Mathf.Min(intentosUsados + 1, intentosMaximos) + " / " + intentosMaximos;
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
