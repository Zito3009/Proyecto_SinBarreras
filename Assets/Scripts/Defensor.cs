using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defensor : MonoBehaviour
{
    [System.Serializable]
    public class Paso
    {
        public int carril = 0;        // -1 = izquierda, 0 = centro, 1 = derecha
        public float espera = 0.1f;
        public float velocidad = 8f;
    }

    [Header("Posición")]
    public int carrilInicial = 0;
    public float separacionCarriles = 3f;   // distancia en X entre carriles

    [Header("Cuándo se mueve (Anticipación)")]
    public float distanciaActivacion = 10f;

    [Header("Movimiento")]
    public Paso paso;

    private Transform jugador;

    // Todos los defensores activos (de todas las líneas)
    private static List<Defensor> activos = new List<Defensor>();

    void OnEnable()
    {
        if (jugador == null)
        {
            CorredorJugador corredor = FindObjectOfType<CorredorJugador>();
            if (corredor != null) jugador = corredor.transform;
        }

        int[] carriles = { -1, 0, 1 };

        // Los defensores que están en la MISMA línea (mismo padre)
        List<Defensor> companeros = new List<Defensor>();
        foreach (Defensor otro in activos)
        {
            if (otro != this && otro.transform.parent == transform.parent)
                companeros.Add(otro);
        }

        // 1. Carril inicial: que no lo use un compañero de la línea
        List<int> libres = new List<int>(carriles);
        foreach (Defensor c in companeros)
            libres.Remove(c.carrilInicial);
        if (libres.Count == 0) libres.AddRange(carriles); // línea con 3 o más: no hay otra

        carrilInicial = libres[Random.Range(0, libres.Count)];
        PonerEnCarril(carrilInicial);

        // 2. Carril destino: distinto al inicial y al destino de los compañeros
        List<int> destinos = new List<int>(carriles);
        destinos.Remove(carrilInicial);
        foreach (Defensor c in companeros)
            destinos.Remove(c.paso.carril);
        if (destinos.Count == 0)
        {
            destinos = new List<int>(carriles);
            destinos.Remove(carrilInicial);
        }

        paso = new Paso();
        paso.carril = destinos[Random.Range(0, destinos.Count)];
        paso.velocidad = 8f;

        // Recién ahora me anoto, así los demás ven mis carriles ya elegidos
        activos.Add(this);

        StartCoroutine(Rutina());
    }

    void OnDisable()
    {
        activos.Remove(this);
    }

    IEnumerator Rutina()
    {
        while (jugador != null && jugador.position.z < transform.position.z - distanciaActivacion)
            yield return null;

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
        return carril * separacionCarriles;
    }

    void PonerEnCarril(int carril)
    {
        Vector3 p = transform.localPosition;
        p.x = XDeCarril(carril);
        transform.localPosition = p;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("jugador") || other.GetComponentInParent<CorredorJugador>() != null)
        {
            CorredorJugador corredor = FindObjectOfType<CorredorJugador>();
            if (corredor != null && corredor.gestor != null)
            {
                corredor.Detener();
                corredor.gestor.TerminarIntento(false);
            }
        }
    }
}