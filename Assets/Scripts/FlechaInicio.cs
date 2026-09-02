using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlechaInicio : MonoBehaviour
{
    public GameObject objetoFlecha;

    private void OnTriggerEnter(Collider other)
    {
        MovimientoDirecto movimiento = other.GetComponent<MovimientoDirecto>();

        if (movimiento != null)
        {
            if (objetoFlecha != null)
            {
                objetoFlecha.SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }
}
