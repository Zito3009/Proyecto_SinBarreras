using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorAgarrar : MonoBehaviour
{
    [Header("Configuración de Alcance")]
    public Transform manoJugador;        // Asigna aquí el punto (objeto vacío) donde irá el objeto
    public float radioAlcance = 2.5f;    // Distancia para detectar objetos
    public KeyCode teclaAgarrar = KeyCode.E;
    public KeyCode teclaSoltar = KeyCode.R;

    public Transform zonaDeposito;
    public float radioDeposito = 2f;
    public ContadorObjetosFase2 contadorFase2;

    private ObjetoAgarrable objetoMirando;
    private ObjetoAgarrable objetoAgarrado;

    void Update()
    {
        // Si no llevamos nada agarrado, buscamos el objeto más cercano
       if (objetoAgarrado == null)
        {
            BuscarObjetosCercanos();

            if (Input.GetKeyDown(teclaAgarrar) && objetoMirando != null)
            {
                AgarrarObjeto();
            }
        }
        else
        {
            if (Input.GetKeyDown(teclaSoltar) && EstaCercaDeZonaDeposito())
            {
                SoltarObjeto();
            }
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
        objetoAgarrado = objetoMirando;
        objetoAgarrado.Agarrar(manoJugador);
        QuitarResaltado();
    }

    void SoltarObjeto()
    {
        objetoAgarrado.Soltar();

        if (objetoAgarrado.PuedeContarseComoDepositado())
        {
            objetoAgarrado.MarcarComoDepositado();
            if (contadorFase2 != null)
                contadorFase2.RegistrarObjetoMovido();
        }
        objetoAgarrado = null; // Liberamos la variable sencillamente
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAlcance);
    }

}
