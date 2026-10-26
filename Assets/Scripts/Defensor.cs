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

        // Coordinamos los carriles de toda la línea a la que pertenece esta esfera
        CoordinarLinea();

        StartCoroutine(Rutina());
    }

    void CoordinarLinea()
    {
        // Si no tiene objeto padre (Linea1, Linea2, etc.), se asigna de forma aleatoria individual
        if (transform.parent == null)
        {
            AsignarAleatorioIndividual();
            return;
        }

        // Obtenemos todos los defensores que son hijos de la misma línea
        Defensor[] defensoresEnLinea = transform.parent.GetComponentsInChildren<Defensor>();

        // Para evitar que cada hijo haga el cálculo por separado, 
        // solo el PRIMER hijo de la lista repartirá los carriles a todos los de su línea
        if (defensoresEnLinea.Length > 0 && defensoresEnLinea[0] == this)
        {
            // Listas de carriles disponibles en la cancha (0 = Izquierda, 1 = Centro, 2 = Derecha)
            List<int> carrilesInicioDisponibles = new List<int> { 0, 1, 2 };
            List<int> carrilesDestinoDisponibles = new List<int> { 0, 1, 2 };

            foreach (Defensor def in defensoresEnLinea)
            {
                // 1. Asignar Carril Inicial (sin repetir entre hermanos)
                int indexInicio = Random.Range(0, carrilesInicioDisponibles.Count);
                def.carrilInicial = carrilesInicioDisponibles[indexInicio];
                carrilesInicioDisponibles.RemoveAt(indexInicio); // Se remueve para que el otro defensor no lo use
                def.PonerEnCarril(def.carrilInicial);

                // 2. Asignar Carril Destino (sin repetir y diferente a su propio carril inicial)
                def.paso = new Paso();
                def.paso.velocidad = 8f;

                // Buscamos destinos disponibles que no sean el mismo carril inicial de este defensor
                List<int> opcionesDestinoValidas = new List<int>();
                foreach (int c in carrilesDestinoDisponibles)
                {
                    if (c != def.carrilInicial)
                    {
                        opcionesDestinoValidas.Add(c);
                    }
                }

                // Si por descarte no hay opción diferente, usamos los que queden
                if (opcionesDestinoValidas.Count == 0)
                {
                    opcionesDestinoValidas = new List<int>(carrilesDestinoDisponibles);
                }

                int carrilElegido = opcionesDestinoValidas[Random.Range(0, opcionesDestinoValidas.Count)];
                def.paso.carril = carrilElegido;
                carrilesDestinoDisponibles.Remove(carrilElegido); // Se remueve para que el otro defensor no vaya ahí
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
        // Se mantiene intacta la distancia de activación: espera a que el jugador se acerque
        while (jugador != null && jugador.position.z < transform.position.z - distanciaActivacion)
            yield return null;

        // Pequeña pausa opcional y movimiento lateral al carril asignado
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
