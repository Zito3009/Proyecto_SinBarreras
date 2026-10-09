using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensorAleatorio : MonoBehaviour
{
    [Header("Posición de Carriles (Eje X)")]
    public float carrilIzquierda = -3f;
    public float carrilCentro = 0f;
    public float carrilDerecha = 3f;

    void OnEnable()
    {
        // Se ejecuta automáticamente cada vez que el Gestor activa la línea
        float[] posicionesX = new float[] { carrilIzquierda, carrilCentro, carrilDerecha };
        int indiceAleatorio = Random.Range(0, posicionesX.Length);

        // Cambia la posición en X sin alterar la altura (Y) ni el avance en la cancha (Z)
        transform.position = new Vector3(posicionesX[indiceAleatorio], transform.position.y, transform.position.z);
    }
}
