using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraTerceraPersona : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 offset = new Vector3(0, 2f, -5f);
    public float velocidadSuavizado = 10f;
    public float sensibilidadRaton = 3f;
    public float limiteVerticalMin = -20f;
    public float limiteVerticalMax = 60f;

    private float rotacionX = 0f;
    private float rotacionY = 0f;

    void LateUpdate()
    {
        if (objetivo == null) return;

        rotacionY += Input.GetAxis("Mouse X") * sensibilidadRaton;
        rotacionX -= Input.GetAxis("Mouse Y") * sensibilidadRaton;
        rotacionX = Mathf.Clamp(rotacionX, limiteVerticalMin, limiteVerticalMax);

        Quaternion rotacion = Quaternion.Euler(rotacionX, rotacionY, 0);
        Vector3 posicionDeseada = objetivo.position + rotacion * offset;

        transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadSuavizado * Time.deltaTime);
        transform.LookAt(objetivo.position + Vector3.up * 1.5f);
    }
}
