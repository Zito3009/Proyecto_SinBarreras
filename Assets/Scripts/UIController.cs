using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Referencia al Backend")]
    public Prueba_Conexion pruebaConexion; // Arrastrá el GameObject con Prueba_Conexion acá

    [Header("Paneles")]
    public GameObject panelInicio;
    public GameObject panelLogin;
    public GameObject panelRegistro;
    public GameObject panelSeleccionPersonaje;

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

    // --- NAVEGACIÓN DE PANELES ---
    public void AbrirLogin()
    {
        panelInicio.SetActive(false);
        panelRegistro.SetActive(false);
        panelLogin.SetActive(true);
    }

    public void AbrirRegistro()
    {
        panelInicio.SetActive(false);
        panelLogin.SetActive(false);
        panelRegistro.SetActive(true);
    }

    public void VolverAlInicio()
    {
        panelInicio.SetActive(true);
        panelLogin.SetActive(false);
        panelRegistro.SetActive(false);
    }

    // --- PUENTE HACIA SUPABASE ---
    // Si tus botones Aceptar ya llamaban a ConfirmarRegistro/ConfirmarLogin, ahora redirigen a Prueba_Conexion
    public void ConfirmarRegistro()
    {
        if (pruebaConexion != null)
        {
            pruebaConexion.OnClickRegistrar();
        }
        else
        {
            Debug.LogError("Falta asignar el componente 'Prueba_Conexion' en el Inspector de UIController.");
        }
    }

    public void ConfirmarLogin()
    {
        if (pruebaConexion != null)
        {
            pruebaConexion.OnClickIniciarSesion();
        }
        else
        {
            Debug.LogError("Falta asignar el componente 'Prueba_Conexion' en el Inspector de UIController.");
        }
    }

    // --- CAMBIO DE PANTALLA TRAS RESPUESTA EXITOSA DE SUPABASE ---
    public void MostrarSeleccionPersonaje()
    {
        panelInicio.SetActive(false);
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