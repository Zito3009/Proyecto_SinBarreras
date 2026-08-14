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
    public Vector3 offset = new Vector3(0, 2f, -5f);
    public float velocidadSuavizado = 10f;

    [Header("Rotación con el Ratón")]
    public bool usarRotacionRaton = true;
    public float sensibilidadRaton = 3f;
    public float limiteVerticalMin = -20f;
    public float limiteVerticalMax = 60f;

    private float rotacionX = 0f;
    private float rotacionY = 0f;

    void Start()
    {
        if (usarRotacionRaton)
        {
            // Bloquea y oculta el cursor en el centro de la pantalla
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void LateUpdate()
    {
        if (objetivo == null) return;

        if (usarRotacionRaton)
        {
            // Lectura del movimiento del ratón
            rotacionY += Input.GetAxis("Mouse X") * sensibilidadRaton;
            rotacionX -= Input.GetAxis("Mouse Y") * sensibilidadRaton;

            // Limitar la inclinación vertical para no voltear la cámara
            rotacionX = Mathf.Clamp(rotacionX, limiteVerticalMin, limiteVerticalMax);

            // Calcular rotación y posición deseada
            Quaternion rotacion = Quaternion.Euler(rotacionX, rotacionY, 0);
            Vector3 posicionDeseada = objetivo.position + rotacion * offset;

            // Movimiento suave hacia la nueva posición
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizado * Time.deltaTime);

            // Apuntar levemente por encima del centro del objetivo (altura de la cabeza/pecho)
            transform.LookAt(objetivo.position + Vector3.up * 1.5f);
        }
        else
        {
            // Seguimiento simple y rígido detrás del personaje sin control de ratón
            Vector3 posicionDeseada = objetivo.position + offset;
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizado * Time.deltaTime);
            transform.LookAt(objetivo.position + Vector3.up * 1.5f);
        }
    }
}