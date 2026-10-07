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
    public GameObject cajaDeposito;
    public GameObject panelFlojoPersonaje2;
    public GameObject panelInstruccionesFase2;
    public int minimoAprobadoFase2 = 10;

    private float tiempo;
    private bool termino;

    private List<ObjetoAgarrable> objetosFase2 = new List<ObjetoAgarrable>();
    private Dictionary<ObjetoAgarrable, Vector3> posicionesOriginales = new Dictionary<ObjetoAgarrable, Vector3>();
    private Dictionary<ObjetoAgarrable, Quaternion> rotacionesOriginales = new Dictionary<ObjetoAgarrable, Quaternion>();

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
        yield return new WaitForSecondsRealtime(5f); 
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

         objetosFase2.Clear();
        posicionesOriginales.Clear();
        rotacionesOriginales.Clear();
        foreach (ObjetoAgarrable obj in FindObjectsOfType<ObjetoAgarrable>())
        {
            objetosFase2.Add(obj);
            posicionesOriginales[obj] = obj.transform.position;
            rotacionesOriginales[obj] = obj.transform.rotation;
        }

        contadorFase2.IniciarFase(); 

        // Reiniciar variables
        tiempo = 0;
        termino = false;

        // Ocultar cursor y reanudar juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;

        if (cajaDeposito != null)
         cajaDeposito.SetActive(true);
    }
    
     public void FinalizarFase2(int objetosMovidos, int maximoObjetos)
    {
        PausarYMostrarCursor();

         if (objetosMovidos < minimoAprobadoFase2)
        {
            panelFlojoPersonaje2.SetActive(true);
        }
        else if (objetosMovidos >= maximoObjetos)
        {
            panelGanasteMejorPersonaje2.SetActive(true);
        }
        else
        {
            panelGanastePersonaje2.SetActive(true);
        }
    }

    public void ReintentarFase2()
    {
        panelFlojoPersonaje2.SetActive(false);
        panelGanastePersonaje2.SetActive(false);
        panelGanasteMejorPersonaje2.SetActive(false);

        foreach (ObjetoAgarrable obj in objetosFase2)
        {
            obj.gameObject.SetActive(true);
            obj.transform.position = posicionesOriginales[obj];
            obj.transform.rotation = rotacionesOriginales[obj];
        }

        controladorAgarrarPersonaje2.ReiniciarBolsillo();

        personaje2.transform.position = posInicio.position;
        contadorFase2.IniciarFase();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

     public void SiguienteNivel()
    {
        Debug.Log("Falta definir la escena del siguiente nivel.");
        // TODO: SceneManager.LoadScene("NombreDelSiguienteNivel");
    }

    void PausarYMostrarCursor()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
}
public void MostrarInstruccionesFase2()
{
    panelMejora.SetActive(false);
    panelInstruccionesFase2.SetActive(true);
}
public void ConfirmarInstruccionesYEmpezarFase2()
{
    panelInstruccionesFase2.SetActive(false);
    EmpezarSegundaPerspectiva();
}
}
