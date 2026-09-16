using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorPrimeraPersona : MonoBehaviour
{
    public CharacterController controller;
    public Transform camara; // la FPSCamera, hija de este objeto

    public float velocidad = 3.5f;
    public float gravedad = 20f;
    public float sensibilidadRaton = 3f;
    public float limiteVerticalMin = -80f;
    public float limiteVerticalMax = 80f;

    private float rotacionVertical = 0f;
    private float velocidadVertical = 0f;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadRaton;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadRaton;

        transform.Rotate(Vector3.up * mouseX);

        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, limiteVerticalMin, limiteVerticalMax);
        if (camara != null)
            camara.localEulerAngles = new Vector3(rotacionVertical, 0, 0);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direccion = (transform.right * horizontal) + (transform.forward * vertical);
        if (direccion.magnitude > 1f) direccion.Normalize();

        if (controller.isGrounded)
            velocidadVertical = -2f;
        else
            velocidadVertical -= gravedad * Time.deltaTime;

        Vector3 movimientoFinal = (direccion * velocidad) + (Vector3.up * velocidadVertical);
        controller.Move(movimientoFinal * Time.deltaTime);
    }
}
