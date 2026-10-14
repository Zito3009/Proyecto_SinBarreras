using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorredorJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;              // metros por segundo hacia adelante
    public float velocidadCambioCarril = 12f; // qué tan rápido se desliza de un carril a otro
    public float separacionCarriles = 3f;     // distancia entre carriles

    [Header("Choque")]
    public float tiempoChoque = 2f;

    [Header("Referencias")]
    public Transform puntoInicio;
    public GameManager2 gestor;

    private CharacterController controller;
    private int carrilActual = 1; // 0 = izquierda, 1 = centro, 2 = derecha
    private bool corriendo;
    private bool chocando;
    private float xCentro;
    private Vector3 ultimoCheckpoint;
    
    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    void Start()
    {
        xCentro = puntoInicio.position.x;
        ColocarEnInicio();
    }

    // La llama el gestor al empezar cada intento
    public void Reiniciar()
    {
        StopAllCoroutines();
        chocando = false;
        ColocarEnInicio();
        corriendo = true;
    }

    // La llama el gestor cuando el intento termina
    public void Detener()
    {
        StopAllCoroutines();
        chocando = false;
        corriendo = false;
    }

    void ColocarEnInicio()
    {
        corriendo = false;
        carrilActual = 1;
        ultimoCheckpoint = puntoInicio.position;
        transform.position = new Vector3(xCentro, transform.position.y, puntoInicio.position.z);
    }

    void Update()
    {
        if (!corriendo || chocando) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            carrilActual = Mathf.Max(0, carrilActual - 1);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            carrilActual = Mathf.Min(2, carrilActual + 1);

        float xObjetivo = xCentro + (carrilActual - 1) * separacionCarriles;
        float x = Mathf.MoveTowards(transform.position.x, xObjetivo, velocidadCambioCarril * Time.deltaTime);
        float z = transform.position.z + velocidad * Time.deltaTime;

        IrA(new Vector3(x, transform.position.y, z));
    }

    // Movimiento normal: con CharacterController usa Move (respeta colisiones), sin él cambia la posición directo
    void IrA(Vector3 destino)
    {
        if (controller != null)
            controller.Move(destino - transform.position);
        else
            transform.position = destino;
    }

    // Salto instantáneo de posición (inicio de intento y vuelta al checkpoint)
    void Teletransportar(Vector3 destino)
    {
        if (controller != null) controller.enabled = false; // un CharacterController activo "pelea" contra el cambio directo de posición
        transform.position = destino;
        if (controller != null) controller.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!corriendo || chocando) return;

        if (other.CompareTag("Defensor"))
        {
            StartCoroutine(Chocar());
            return;
        }

        ZonaDescanso zona = other.GetComponent<ZonaDescanso>();
        if (zona != null)
        {
            ultimoCheckpoint = zona.transform.position;

            if (zona.numeroLinea >= gestor.lineasActivas)
            {
                corriendo = false;
                gestor.LlegoAlFinal();
            }
        }
    }

    IEnumerator Chocar()
    {
        chocando = true;
        yield return new WaitForSeconds(tiempoChoque);

        carrilActual = 1;
        Teletransportar(new Vector3(xCentro, transform.position.y, ultimoCheckpoint.z));
        chocando = false;
    }
}
