using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaDepositoFase2 : MonoBehaviour
{
public ContadorObjetosFase2 contador;

    private void OnTriggerEnter(Collider other)
    {
        ObjetoAgarrable objeto = other.GetComponent<ObjetoAgarrable>();
        if (objeto != null && objeto.PuedeContarseComoDepositado())
        {
            objeto.MarcarComoDepositado();
            contador.RegistrarObjetoMovido();
        }
    }
}
