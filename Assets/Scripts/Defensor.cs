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

    // Memoria global compartida entre TODAS las líneas del juego
    private static int ultimoCarrilGlobal = -1;

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

        CoordinarLinea();

        StartCoroutine(Rutina());
    }

    void CoordinarLinea()
    {
        if (transform.parent == null)
        {
            AsignarAleatorioIndividual();
            return;
        }

        Defensor[] defensoresEnLinea = transform.parent.GetComponentsInChildren<Defensor>();

        if (defensoresEnLinea.Length > 0 && defensoresEnLinea[0] == this)
        {
            List<int> carrilesInicioDisponibles = new List<int> { 0, 1, 2 };
            List<int> carrilesDestinoDisponibles = new List<int> { 0, 1, 2 };

            for (int i = 0; i < defensoresEnLinea.Length; i++)
            {
                Defensor def = defensoresEnLinea[i];

                // --- 1. CARRIL INICIAL ---
                // Si es la PRIMERA esfera de esta línea, evitamos el carril de la línea anterior
                List<int> opcionesInicioValidas = new List<int>();
                foreach (int c in carrilesInicioDisponibles)
                {
                    if (i == 0 && c == ultimoCarrilGlobal && carrilesInicioDisponibles.Count > 1)
                    {
                        continue; // Descarta el carril de la línea anterior solo para el primer defensor
                    }
                    opcionesInicioValidas.Add(c);
                }

                int carrilElegidoInicio = opcionesInicioValidas[Random.Range(0, opcionesInicioValidas.Count)];
                def.carrilInicial = carrilElegidoInicio;
                carrilesInicioDisponibles.Remove(carrilElegidoInicio);
                def.PonerEnCarril(def.carrilInicial);

                // --- 2. CARRIL DESTINO ---
                def.paso = new Paso();
                def.paso.velocidad = 8f;

                List<int> opcionesDestinoValidas = new List<int>();
                foreach (int c in carrilesDestinoDisponibles)
                {
                    if (c != def.carrilInicial)
                    {
                        opcionesDestinoValidas.Add(c);
                    }
                }

                if (opcionesDestinoValidas.Count == 0)
                {
                    opcionesDestinoValidas = new List<int>(carrilesDestinoDisponibles);
                }

                int carrilElegidoDestino = opcionesDestinoValidas[Random.Range(0, opcionesDestinoValidas.Count)];
                def.paso.carril = carrilElegidoDestino;
                carrilesDestinoDisponibles.Remove(carrilElegidoDestino);

                // Guardamos la última posición asignada para que la PRÓXIMA línea sepa cuál evitar
                ultimoCarrilGlobal = def.carrilInicial;
            }
        }
    }

    void AsignarAleatorioIndividual()
    {
        carrilInicial = Random.Range(0, 3);
        PonerEnCarril(carrilInicial);

        paso = new Paso();
        do
        {
            paso.carril = Random.Range(0, 3);
        }
        while (paso.carril == carrilInicial);

        paso.velocidad = 8f;
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
        return (carril - 1) * separacionCarriles;
    }

    void PonerEnCarril(int carril)
    {
        Vector3 p = transform.localPosition;
        p.x = XDeCarril(carril);
        transform.localPosition = p;
    }
}
