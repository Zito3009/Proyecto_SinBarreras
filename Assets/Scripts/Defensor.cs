using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defensor : MonoBehaviour
{
    [System.Serializable]
    public class Paso
    {
        public int carril = 0;        // -3 = izquierda, 0 = centro, 3 = derecha
        public float espera = 0.1f;   // tiempo mínimo de espera
        public float velocidad = 8f;  // qué tan rápido se desplaza lateralmente
    }

    [Header("Posición")]
    public int carrilInicial = 0;
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

        // Opciones de carriles fijas en las coordenadas X
        int[] carriles = { -3, 0, 3 };

        // 1. Ubicación inicial aleatoria (-3, 0 o 3)
        carrilInicial = carriles[Random.Range(0, carriles.Length)];
        PonerEnCarril(carrilInicial);

        // 2. Elige un carril destino aleatorio DIFERENTE al inicial
        paso = new Paso();
        do
        {
            paso.carril = carriles[Random.Range(0, carriles.Length)];
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

        // Pausa diminuta y movimiento directo
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
        // Al ser -3, 0 o 3, el parámetro ya es la coordenada en X
        return carril;
    }

    void PonerEnCarril(int carril)
    {
        Vector3 p = transform.localPosition;
        p.x = XDeCarril(carril);
        transform.localPosition = p;
    }
}