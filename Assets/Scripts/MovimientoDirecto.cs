using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovimientoDirecto : MonoBehaviour
{
    [Header("Componentes")]
    public CharacterController controller;
    [Tooltip("Arrastra aquí la cámara principal")]
    public Transform camara;

    [Header("Ajustes de Movimiento")]
    public float velocidad = 6f;
    public float gravedad = 20f;
    
    [Tooltip("Velocidad de giro del personaje. Más bajo = más lento/pesado.")]
    public float suavizadoGiroPersonaje = 10f; 

    private float velocidadVertical = 0f;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        // Intenta buscar la cámara principal si no se asignó
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;

        if (camara == null)
            Debug.LogError("Por favor, asigna la Cámara en el Inspector del objeto " + gameObject.name);
    }

    void Update()
    {
        if (camara == null) return;

        // 1. Lectura de WASD / Flechas
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direccionInput = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 direccionDeseadaMovimiento = Vector3.zero;

        // 2. Calcular dirección respecto a la vista actual de la cámara
        if (direccionInput.magnitude >= 0.1f)
        {
            // Obtener vectores de la cámara aplanados (Y=0)
            Vector3 camForward = camara.forward;
            Vector3 camRight = camara.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // Dirección final de movimiento
            direccionDeseadaMovimiento = (camForward * direccionInput.z) + (camRight * direccionInput.x);

            // 3. Rotación SUAVE del personaje hacia donde camina
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionDeseadaMovimiento);
            // Usamos Slerp para que el giro del modelo sea gomoso y no un tirón
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, suavizadoGiroPersonaje * Time.deltaTime);
        }

        // 4. Control de gravedad y suelo
        if (controller.isGrounded)
        {
            velocidadVertical = -2f; // Mantener pegado al suelo
        }
        else
        {
            velocidadVertical -= gravedad * Time.deltaTime;
        }

        // 5. Aplicar movimiento final
        Vector3 movimientoFinal = (direccionDeseadaMovimiento * velocidad) + (Vector3.up * velocidadVertical);
        controller.Move(movimientoFinal * Time.deltaTime);
    }
}