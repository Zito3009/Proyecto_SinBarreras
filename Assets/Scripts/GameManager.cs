using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Paneles de la Fase 1")]
    public GameObject panelPerdiste;
    public GameObject panelGanaste;
    public GameObject panelGanasteMejor;
    public GameObject panelMejora;

    [Header("Personajes y Posiciones")]
    public GameObject personaje1;
    public GameObject personaje2;
    public Transform posInicio;

    [Header("Paneles de la Fase 2")]
    public GameObject panelGanastePersonaje2;
    public GameObject panelGanasteMejorPersonaje2;
    public GameObject panelFlojoPersonaje2;
    public GameObject panelInstruccionesFase2;

    [Header("Referencias de la Fase 2")]
    public CamaraTerceraPersona camaraJugador;
    public ControladorAgarrar controladorAgarrarPersonaje2;
    public ContadorObjetosFase2 contadorFase2;
    public GameObject panelContador;
    public GameObject flecha;
    public GameObject flechaFase2;
    public Animator puerta;
    public GameObject cajaDeposito;
    public int minimoAprobadoFase2 = 10;

    [Header("Transición")]
    public Transicion_niveles transicionNiveles;

    private float tiempo;
    private bool termino;
    private bool esFase2 = false;

    private List<ObjetoAgarrable> objetosFase2 = new List<ObjetoAgarrable>();
    private Dictionary<ObjetoAgarrable, Vector3> posicionesOriginales = new Dictionary<ObjetoAgarrable, Vector3>();
    private Dictionary<ObjetoAgarrable, Quaternion> rotacionesOriginales = new Dictionary<ObjetoAgarrable, Quaternion>();

    void Update()
    {
        if (!termino && Time.timeScale > 0)
            tiempo += Time.deltaTime;
    }

    public void TiempoAgotado()
    {
        termino = true;
        PausarYMostrarCursor();
        
        if (esFase2)
        {
            if (panelFlojoPersonaje2 != null)
            {
                panelFlojoPersonaje2.SetActive(true);
            }
        }
        else
        {
            if (panelPerdiste != null)
            {
                panelPerdiste.SetActive(true);
            }
        }
    }

    // Se ejecuta al tocar la puerta en la Fase 1
    public void TocarPuerta()
    {
        if (termino) return;
        termino = true;

        StartCoroutine(RutinaVictoriaFase1());
    }

    IEnumerator RutinaVictoriaFase1()
    {
        // 1. Inicia el fundido a negro progresivo
        if (transicionNiveles != null)
        {
            transicionNiveles.AparecerDeAPoco();
            // Espera a que termine la animación de fundido
            yield return new WaitForSecondsRealtime(transicionNiveles.duracionFundido);
        }

        // 2. Muestra el cartel de victoria con la pantalla ya en negro
        PausarYMostrarCursor();

        if (tiempo < 40f)
            panelGanasteMejor.SetActive(true);
        else
            panelGanaste.SetActive(true);

        // 3. Espera 5 segundos antes de desplegar el menú de mejora
        yield return new WaitForSecondsRealtime(5f);

        panelGanaste.SetActive(false);
        panelGanasteMejor.SetActive(false);
        panelMejora.SetActive(true);
    }

    public void MostrarInstruccionesFase2()
    {
        panelMejora.SetActive(false);
        if (panelInstruccionesFase2 != null)
            panelInstruccionesFase2.SetActive(true);
    }

    public void ConfirmarInstruccionesYEmpezarFase2()
    {
        if (panelInstruccionesFase2 != null)
            panelInstruccionesFase2.SetActive(false);

        EmpezarSegundaPerspectiva();
    }

    public void EmpezarSegundaPerspectiva()
    {
        // Aclara la pantalla para comenzar la Fase 2
        if (transicionNiveles != null)
        {
            transicionNiveles.DesaparecerDeAPoco();
        }

        panelMejora.SetActive(false);

        if (flecha != null)
            flecha.SetActive(false);

        if (flechaFase2 != null)
            flechaFase2.SetActive(true);
           
        if (puerta != null)
        {
            puerta.Play("Abrir", 0, 0f);   // Fuerza el frame 0 (puerta cerrada)
            puerta.Update(0f);              // Aplica esa pose de inmediato
            puerta.enabled = false;         // Congela la puerta cerrada
        }

        if (panelContador != null)
            panelContador.SetActive(true);

        // Cambiar personajes y posicionar al segundo en el inicio
        if (personaje1 != null) personaje1.SetActive(false);
        if (personaje2 != null)
        {
            personaje2.transform.position = posInicio.position;
            personaje2.SetActive(true);
        }

        if (panelGanaste != null) panelGanaste.SetActive(false);
        if (panelGanasteMejor != null) panelGanasteMejor.SetActive(false);
        if (panelPerdiste != null) panelPerdiste.SetActive(false);

        if (controladorAgarrarPersonaje2 != null)
            controladorAgarrarPersonaje2.enabled = true;

        // Guardar posiciones de los objetos agarrables de la escena
        objetosFase2.Clear();
        posicionesOriginales.Clear();
        rotacionesOriginales.Clear();
        foreach (ObjetoAgarrable obj in FindObjectsOfType<ObjetoAgarrable>())
        {
            objetosFase2.Add(obj);
            posicionesOriginales[obj] = obj.transform.position;
            rotacionesOriginales[obj] = obj.transform.rotation;
        }

        if (contadorFase2 != null)
            contadorFase2.IniciarFase();

        // Reiniciar variables
        tiempo = 0;
        termino = false;
        esFase2 = true;

        // Ocultar cursor y reanudar el juego
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        if (cajaDeposito != null)
            cajaDeposito.SetActive(true);
    }

    public void FinalizarFase2(int objetosMovidos, int maximoObjetos)
    {
        PausarYMostrarCursor();

        if (objetosMovidos >= minimoAprobadoFase2 && transicionNiveles != null)
        {
            transicionNiveles.AparecerDeAPoco();
        }

        if (objetosMovidos < minimoAprobadoFase2)
        {
            if (panelFlojoPersonaje2 != null) panelFlojoPersonaje2.SetActive(true);
        }
        else if (objetosMovidos >= maximoObjetos)
        {
            if (panelGanasteMejorPersonaje2 != null) panelGanasteMejorPersonaje2.SetActive(true);
        }
        else 
        {
            if (panelGanastePersonaje2 != null) panelGanastePersonaje2.SetActive(true);
        }
    }

    public void ReintentarFase2()
    {
        // Remueve la pantalla negra al reiniciar
        if (transicionNiveles != null)
        {
            transicionNiveles.DesaparecerDeAPoco();
        }

        if (panelFlojoPersonaje2 != null) panelFlojoPersonaje2.SetActive(false);
        if (panelGanastePersonaje2 != null) panelGanastePersonaje2.SetActive(false);
        if (panelGanasteMejorPersonaje2 != null) panelGanasteMejorPersonaje2.SetActive(false);

        foreach (ObjetoAgarrable obj in objetosFase2)
        {
            obj.gameObject.SetActive(true);
            obj.transform.position = posicionesOriginales[obj];
            obj.transform.rotation = rotacionesOriginales[obj];
        }

        if (controladorAgarrarPersonaje2 != null)
            controladorAgarrarPersonaje2.ReiniciarBolsillo();

        if (personaje2 != null)
            personaje2.transform.position = posInicio.position;

        if (contadorFase2 != null)
            contadorFase2.IniciarFase();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void SiguienteNivel()
    {
        Debug.Log("Falta definir la escena del siguiente nivel.");
        // SceneManager.LoadScene("NombreDelSiguienteNivel");
    }

    void PausarYMostrarCursor()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}