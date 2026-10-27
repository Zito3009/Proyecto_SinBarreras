using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defensor : MonoBehaviour
{
   [System.Serializable]
    public class Paso
    {
        public int carril = 1;        // 0 = izquierda, 1 = centro, 2 = derecha
        public float espera = 0.1f;   // tiempo mínimo de espera
        public float velocidad = 8f;  // qué tan rápido se desplaza lateralmente
    }

    [Header("Posición")]
    public int carrilInicial = 1;
    public float separacionCarriles = 3f;

    [Header("Cuándo se mueve (Anticipación)")]
    [Tooltip("Distancia en Z a la que el defensor inicia el movimiento al ver al jugador.")]
    public float distanciaActivacion = 10f; 

    [Header("Movimiento")]
    public Paso paso;

    private Transform jugador;
    void OnEnable()
    {
        if (jugador == null)
        {
            CorredorJugador corredor = FindObjectOfType<CorredorJugador>();
            if (corredor != null) jugador = corredor.transform;
        }

        // 1. Ubicación inicial aleatoria (0, 1 o 2)
        carrilInicial = Random.Range(0, 3);
        PonerEnCarril(carrilInicial);

        // 2. Elige un carril destino aleatorio DIFERENTE al inicial
        paso = new Paso();
        do
        {
            paso.carril = Random.Range(0, 3);
        } 
        while (paso.carril == carrilInicial);

        paso.velocidad = 8f;

        StartCoroutine(Rutina());
    }

    IEnumerator Rutina()
    {
        // Espera a que el jugador esté a la distancia de activación
        while (jugador != null && jugador.position.z < transform.position.z - distanciaActivacion)
            yield return null;

        // Pausa diminuta y movimiento directo sin cambiar de color
        yield return new WaitForSeconds(paso.espera);
        yield return StartCoroutine(IrAX(XDeCarril(paso.carril), paso.velocidad));
    }

    IEnumerator IrAX(float xObjetivo, float velocidad)
    {
        while (Mathf.Abs(transform.localPosition.x - xObjetivo) > 0.01f)
        {
            Vector3 p = transform.localPosition;
            p.x = Mathf.MoveTowards(p.x, xObjetivo, velocidad * Time.deltaTime);
            transform.localPosition = p;
            yield return null;
        }
    }

    float XDeCarril(int carril)
    {
        return (carril - 1) * separacionCarriles;
    }

    void PonerEnCarril(int carril)
    {
        Vector3 p = transform.localPosition;
        p.x = XDeCarril(carril);
        transform.localPosition = p;
    }
}