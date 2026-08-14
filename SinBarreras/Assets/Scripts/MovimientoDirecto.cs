using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoDirecto : MonoBehaviour
{
    [Header("Componentes")]
    public CharacterController controller;
    public Transform camara;

    [Header("Ajustes de Movimiento")]
    public float velocidad = 6f;
    public float gravedad = 20f;
    public float velocidadRotacion = 12f; // Velocidad con la que el personaje gira a donde camina

    private float velocidadVertical = 0f;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        // Si no asignas la cámara manualmente, intenta buscar la cámara principal
        if (camara == null && Camera.main != null)
            camara = Camera.main.transform;
    }

    void Update()
    {
        if (camara == null) return;

        // 1. Lectura de WASD / Flechas
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 2. Obtener los vectores 'Adelante' y 'Derecha' de la CÁMARA
        Vector3 camForward = camara.forward;
        Vector3 camRight = camara.right;

        // Aplanar los vectores (Y = 0) para que mirar hacia arriba/abajo con la cámara
        // no haga que el personaje intente volar o enterrarse en el suelo.
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // 3. Calcular la dirección respecto a la vista actual de la cámara
        Vector3 direccionDeseada = (camForward * vertical) + (camRight * horizontal);

        // Evitar mayor velocidad al presionar en diagonal (W + D)
        if (direccionDeseada.magnitude > 1f)
        {
            direccionDeseada.Normalize();
        }

        // 4. Rotar el personaje para que mire hacia donde camina
        if (direccionDeseada.magnitude > 0.1f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionDeseada);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }

        // 5. Control de gravedad y suelo
        if (controller.isGrounded)
        {
            velocidadVertical = -2f; // Mantener pegado al suelo
        }
        else
        {
            velocidadVertical -= gravedad * Time.deltaTime;
        }

        // 6. Aplicar movimiento final
        Vector3 movimientoFinal = (direccionDeseada * velocidad) + (Vector3.up * velocidadVertical);
        controller.Move(movimientoFinal * Time.deltaTime);
    }
}