using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelInicio;
    public GameObject panelLogin;
    public GameObject panelRegistro;
    public GameObject panelSeleccionPersonaje;

    [Header("Inputs Registro")]
    public TMP_InputField registroUsuarioOMail;
    public TMP_InputField registroContrasena;

    [Header("Inputs Login")]
    public TMP_InputField loginNombre;
    public TMP_InputField loginApellido;
    public TMP_InputField loginMail;
    public TMP_InputField loginUsuario;
    public TMP_InputField loginContrasena;

    void Start()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        panelInicio.SetActive(true);
        panelLogin.SetActive(false);
        panelRegistro.SetActive(false);
        panelSeleccionPersonaje.SetActive(false);
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

    public void ConfirmarRegistro()
    {
        if (string.IsNullOrEmpty(registroUsuarioOMail.text) || string.IsNullOrEmpty(registroContrasena.text))
        {
            Debug.Log("Completá usuario/mail y contraseña.");
            return;
        }
        MostrarSeleccionPersonaje();
    }

    public void ConfirmarLogin()
    {
        bool tieneMailOUsuario = !string.IsNullOrEmpty(loginMail.text) || !string.IsNullOrEmpty(loginUsuario.text);

        if (string.IsNullOrEmpty(loginNombre.text) || string.IsNullOrEmpty(loginApellido.text) ||
            !tieneMailOUsuario || string.IsNullOrEmpty(loginContrasena.text))
        {
            Debug.Log("Completá todos los campos requeridos.");
            return;
        }
        MostrarSeleccionPersonaje();
    }

    void MostrarSeleccionPersonaje()
    {
        panelLogin.SetActive(false);
        panelRegistro.SetActive(false);
        panelSeleccionPersonaje.SetActive(true);
    }

    public void EmpezarJuego()
    {
        panelSeleccionPersonaje.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}