using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorAgarrar : MonoBehaviour
{
    [Header("Configuracion de Alcance")]
    public Transform manoJugador;        // Objeto vacio 'Mano'
    public float radioAlcance = 2.5f;    // Distancia alrededor de la esfera donde detecta objetos
    public KeyCode teclaAgarrar = KeyCode.E;

    private ObjetoAgarrable objetoMirando;
    private ObjetoAgarrable objetoAgarrado;

    void Update()
    {
        // Si no tenemos nada agarrado, buscamos si hay un objeto cerca
        if (objetoAgarrado == null)
        {
            BuscarObjetosCercanos();
        }

        // Tecla para agarrar / soltar
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
        // Detecta todos los colisionadores en un radio circular alrededor del personaje
        Collider[] objetosEncontrados = Physics.OverlapSphere(transform.position, radioAlcance);

        ObjetoAgarrable objetoMasCercano = null;
        float menorDistancia = float.MaxValue;

        foreach (Collider col in objetosEncontrados)
        {
            // Ignoramos si se choca a si mismo (la esfera del jugador)
            if (col.gameObject == gameObject) continue;

            ObjetoAgarrable agarrable = col.GetComponent<ObjetoAgarrable>();

            if (agarrable != null)
            {
                // Calculamos cual es el objeto mas cercano si hay varios
                float distancia = Vector3.Distance(transform.position, agarrable.transform.position);
                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    objetoMasCercano = agarrable;
                }
            }
        }

        // Si encontramos un objeto interactuable cercano
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

        // Posiciona el objeto un poco por delante del personaje sobre el suelo
        objetoAgarrado.transform.position = transform.position + transform.forward * 1.2f;
        
        objetoAgarrado = null;
    }

    // Dibuja la esfera de alcance en rojo dentro de la ventana 'Scene' para que veas el radio de kilometraje
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioAlcance);
    }
}
