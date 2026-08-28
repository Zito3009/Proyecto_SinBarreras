using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorAgarrar : MonoBehaviour
{
    [Header("Configuración de Alcance")]
    public Transform manoJugador;        // Asigna aquí el punto (objeto vacío) donde irá el objeto
    public float radioAlcance = 2.5f;    // Distancia para detectar objetos
    public KeyCode teclaAgarrar = KeyCode.E;

    private ObjetoAgarrable objetoMirando;
    private ObjetoAgarrable objetoAgarrado;

    void Update()
    {
        // Si no llevamos nada agarrado, buscamos el objeto más cercano
        if (objetoAgarrado == null)
        {
            BuscarObjetosCercanos();
        }

        // Tecla para interactuar
        if (Input.GetKeyDown(teclaAgarrar))
        {
            if (objetoAgarrado != null)
            {
                SoltarObjeto();
            }
            else if (objetoMirando != null)
            {
                AgarrarObjeto();
            }
        }
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
        objetoAgarrado = null; // Liberamos la variable sencillamente
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAlcance);
    }
}
