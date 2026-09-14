using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeleccionPersonaje : MonoBehaviour
{
    public GameObject panelSeleccion;
    public GameObject panelHistoria;
    public UIController uiController;

    public void SeleccionarPersonaje1()
    {
        panelSeleccion.SetActive(false);
        panelHistoria.SetActive(true);
    }

    public void ConfirmarHistoriaYEmpezar()
    {
        panelHistoria.SetActive(false);
        uiController.EmpezarJuego();
    }
}
