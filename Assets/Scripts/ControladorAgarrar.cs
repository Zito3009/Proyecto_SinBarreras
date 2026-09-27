using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControladorAgarrar : MonoBehaviour
{
    [Header("Configuración de Alcance")]
    public float radioAlcance = 2.5f;
    public KeyCode teclaAgarrar = KeyCode.E;
    public KeyCode teclaSoltar = KeyCode.R;

    [Header("Bolsillo")]
    public int capacidadBolsillo = 4;

    [Header("Advertencia de bolsillo lleno")]
    public GameObject panelAdvertencia;
    public float duracionAdvertencia = 1f;

    [Header("Zona de Depósito")]
    public Transform zonaDeposito;
    public float radioDeposito = 2f;
    public ContadorObjetosFase2 contadorFase2;

    private ObjetoAgarrable objetoMirando;
    private List<ObjetoAgarrable> objetosEnBolsillo = new List<ObjetoAgarrable>();
    private bool mostrandoAdvertencia = false;

    void Start()
    {
        if (panelAdvertencia != null)
            panelAdvertencia.SetActive(false);
    }

    void Update()
    {
        if (mostrandoAdvertencia) return; // congelado, no procesar nada más

        if (objetosEnBolsillo.Count < capacidadBolsillo)
        {
            BuscarObjetosCercanos();

            if (Input.GetKeyDown(teclaAgarrar) && objetoMirando != null)
            {
                AgarrarObjeto();
            }
        }
        else
        {
            QuitarResaltado();
        }

        if (Input.GetKeyDown(teclaSoltar) && objetosEnBolsillo.Count > 0 && EstaCercaDeZonaDeposito())
        {
            DepositarTodo();
        }
    }

    bool EstaCercaDeZonaDeposito()
    {
        if (zonaDeposito == null) return false;
        return Vector3.Distance(transform.position, zonaDeposito.position) <= radioDeposito;
    }

    void BuscarObjetosCercanos()
    {
        Collider[] objetosEncontrados = Physics.OverlapSphere(transform.position, radioAlcance);
        ObjetoAgarrable objetoMasCercano = null;
        float menorDistancia = float.MaxValue;

        foreach (Collider col in objetosEncontrados)
        {
            if (col.gameObject == gameObject) continue;
            ObjetoAgarrable agarrable = col.GetComponent<ObjetoAgarrable>();
            if (agarrable != null)
            {
                float distancia = Vector3.Distance(transform.position, agarrable.transform.position);
                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    objetoMasCercano = agarrable;
                }
            }
        }

        if (objetoMasCercano != null)
        {
            if (objetoMirando != objetoMasCercano)
            {
                QuitarResaltado();
                objetoMirando = objetoMasCercano;
                objetoMirando.Resaltar(true);
            }
        }
        else
        {
            QuitarResaltado();
        }
    }

    void QuitarResaltado()
    {
        if (objetoMirando != null)
        {
            objetoMirando.Resaltar(false);
            objetoMirando = null;
        }
    }

    void AgarrarObjeto()
    {
        ObjetoAgarrable objeto = objetoMirando;
        QuitarResaltado();

        objeto.Guardar();
        objetosEnBolsillo.Add(objeto);

        if (objetosEnBolsillo.Count >= capacidadBolsillo)
        {
            StartCoroutine(MostrarAdvertenciaBolsilloLleno());
        }
    }

    IEnumerator MostrarAdvertenciaBolsilloLleno()
    {
        mostrandoAdvertencia = true;

        if (panelAdvertencia != null)
            panelAdvertencia.SetActive(true);

        float tiempoAnterior = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duracionAdvertencia);

        Time.timeScale = tiempoAnterior;

        if (panelAdvertencia != null)
            panelAdvertencia.SetActive(false);

        mostrandoAdvertencia = false;
    }

    void DepositarTodo()
    {
        int cantidad = objetosEnBolsillo.Count;

        foreach (ObjetoAgarrable objeto in objetosEnBolsillo)
        {
            objeto.Depositar();
        }
        objetosEnBolsillo.Clear();

        if (contadorFase2 != null)
        {
            for (int i = 0; i < cantidad; i++)
                contadorFase2.RegistrarObjetoMovido();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAlcance);

        if (zonaDeposito != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(zonaDeposito.position, radioDeposito);
        }
    }

}
