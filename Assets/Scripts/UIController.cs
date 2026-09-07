using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject panelInicio;
    public GameObject panelLogin;
    public GameObject panelRegistro;

    void Start()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void AbrirLogin()
    {
        panelInicio.SetActive(false);
        panelLogin.SetActive(true);
    }

    public void AbrirRegistro()
    {
        panelInicio.SetActive(false);
        panelRegistro.SetActive(true);
    }

    public void VolverAlInicio()
    {
        panelInicio.SetActive(true);
        panelLogin.SetActive(false);
        panelRegistro.SetActive(false);
    }
    public void EmpezarJuego()
    {
        panelInicio.SetActive(false);
        panelLogin.SetActive(false);
        panelRegistro.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }
}
