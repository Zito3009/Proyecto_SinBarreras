using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraTerceraPersona : MonoBehaviour
{
    [Header("Objetivo")]
    [Tooltip("Arrastra aquí el Transform de tu personaje")]
    public Transform objetivo;

    [Header("Posición y Distancia")]
    [Tooltip("Distancia respecto al jugador (X: lado, Y: altura, Z: distancia detrás)")]
    public Vector3 offset = new Vector3(0f, 2.5f, -6f); // Aumentado un poco para mejor visión
    
    [Tooltip("Velocidad de suavizado de POSICIÓN. Más bajo = más lento sigue.")]
    public float suavizadoPosicion = 8f;

    [Header("Rotación con el Ratón (Orbitar)")]
    public float sensibilidadRaton = 3f;
    public float limiteVerticalMin = -20f; // No mirar tan abajo
    public float limiteVerticalMax = 50f;  // No mirar tan arriba

    private float rotacionX = 0f;
    private float rotacionY = 0f;

    void Start()
    {
        // Bloquea y oculta el cursor
        Cursor.lockState = CursorLockMode.Locked;

        if (objetivo == null)
        {
            Debug.LogError("Por favor, asigna el Objetivo (Personaje) en el Inspector de la Cámara.");
            return;
        }

        // Inicializar rotación basada en la posición actual para evitar saltos al inicio
        Vector3 angulosActuales = transform.eulerAngles;
        rotacionY = angulosActuales.y;
        rotacionX = angulosActuales.x;
    }

    // LateUpdate es vital para cámaras, corre DESPUÉS de que el personaje se mueva
    void LateUpdate()
    {
        if (objetivo == null) return;

        // 1. Entrada del ratón
        rotacionY += Input.GetAxis("Mouse X") * sensibilidadRaton;
        rotacionX -= Input.GetAxis("Mouse Y") * sensibilidadRaton;

        // Limitar inclinación vertical
        rotacionX = Mathf.Clamp(rotacionX, limiteVerticalMin, limiteVerticalMax);

        // 2. Calcular la rotación DESEADA basada SOLO en el ratón
        Quaternion rotacionOrbita = Quaternion.Euler(rotacionX, rotacionY, 0);

        // 3. Calcular posición deseada (Pivotando alrededor del objetivo)
        // Usamos un pequeño offset vertical extra en el objetivo (Vector3.up * 1f) 
        // para apuntar al pecho/cabeza, no a los pies, sin mover el offset real.
        Vector3 puntoPivot = objetivo.position + Vector3.up * 1f;
        Vector3 posicionFinalDeseada = puntoPivot + (rotacionOrbita * offset);

        // 4. Aplicar suavizado SOLO a la POSICIÓN
        transform.position = Vector3.Lerp(transform.position, posicionFinalDeseada, suavizadoPosicion * Time.deltaTime);

        // 5. Aplicar la rotación DIRECTAMENTE de la órbita (ADIÓS LookAt instantáneo)
        // Al asignar directamente la rotación del ratón, la cámara se siente sólida y sin tirones.
        transform.rotation = rotacionOrbita;
    }
}