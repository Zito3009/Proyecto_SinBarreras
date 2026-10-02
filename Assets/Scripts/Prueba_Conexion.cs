using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

// Estructuras de datos
[System.Serializable]
public class LogroData
{
    public string user_id;
    public string nombre_logro;
}

[System.Serializable]
public class SupabaseUser
{
    public string id;
}

[System.Serializable]
public class SupabaseAuthResponse
{
    public string access_token;
    public SupabaseUser user;
}

// Estructuras para enviar Nombre y Apellido a Supabase
[System.Serializable]
public class UserMetaData
{
    public string nombre;
    public string apellido;
}

[System.Serializable]
public class SignUpRequest
{
    public string email;
    public string password;
    public UserMetaData data;
}

public class Prueba_Conexion : MonoBehaviour
{
    [Header("Credenciales Supabase")]
    public string supabaseUrl = "https://dwnovgbnqydetvjxfcmm.supabase.co";
    public string supabaseApiKey = "sb_publishable_DsiebBzA1MR1wdgT3yzXiQ_HkkYrCqE";

    [Header("Paneles de UI")]
    public GameObject Pantalla_Iniciosesion;
    public GameObject Pantalla_Login;

    [Header("UI Registro (Crear Cuenta)")]
    public TMP_InputField registroNombreInput;    // Campo Nombre
    public TMP_InputField registroApellidoInput;  // Campo Apellido
    public TMP_InputField registroMailInput;
    public TMP_InputField registroPasswordInput;
    public TextMeshProUGUI registroMensajeText;

    [Header("UI Inicio de Sesión")]
    public TMP_InputField loginMailInput;
    public TMP_InputField loginPasswordInput;
    public TextMeshProUGUI loginExitoText;
    public TextMeshProUGUI loginErrorText;

    void Start()
    {
        OcultarMensajes();
    }

    void OcultarMensajes()
    {
        if (registroMensajeText != null) registroMensajeText.gameObject.SetActive(false);
        if (loginExitoText != null) loginExitoText.gameObject.SetActive(false);
        if (loginErrorText != null) loginErrorText.gameObject.SetActive(false);
    }

    public void OnClickRegistrar()
    {
        OcultarMensajes();
        StartCoroutine(RegistrarUsuarioCorrutina(
            registroMailInput.text, 
            registroPasswordInput.text,
            registroNombreInput.text,
            registroApellidoInput.text
        ));
    }

    public void OnClickIniciarSesion()
    {
        OcultarMensajes();
        StartCoroutine(IniciarSesionCorrutina(loginMailInput.text, loginPasswordInput.text));
    }

    IEnumerator RegistrarUsuarioCorrutina(string email, string password, string nombre, string apellido)
    {
        string url = supabaseUrl + "/auth/v1/signup";

        // Creamos la estructura con email, clave y metadatos
        SignUpRequest body = new SignUpRequest
        {
            email = email,
            password = password,
            data = new UserMetaData
            {
                nombre = nombre,
                apellido = apellido
            }
        };

        string jsonBody = JsonUtility.ToJson(body);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", supabaseApiKey);

            yield return request.SendWebRequest();

            registroMensajeText.gameObject.SetActive(true);

            if (!request.isNetworkError && !request.isHttpError)
            {
                registroMensajeText.text = "¡Cuenta creada con éxito!";
            }
            else
            {
                registroMensajeText.text = "Error al crear cuenta.";
                Debug.LogError("Error registro: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator IniciarSesionCorrutina(string email, string password)
    {
        string url = supabaseUrl + "/auth/v1/token?grant_type=password";
        string jsonBody = $"{{\"email\":\"{email}\",\"password\":\"{password}\"}}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", supabaseApiKey);

            yield return request.SendWebRequest();

            if (!request.isNetworkError && !request.isHttpError)
            {
                loginExitoText.gameObject.SetActive(true);

                // 1. Extraer el ID de usuario devuelto por Supabase
                SupabaseAuthResponse authData = JsonUtility.FromJson<SupabaseAuthResponse>(request.downloadHandler.text);
                string userId = authData.user.id;

                // 2. Guardar el logro en la tabla logros_jugador
                StartCoroutine(GuardarLogroCorrutina(userId, "Primer Inicio de Sesión"));
            }
            else
            {
                loginErrorText.gameObject.SetActive(true);
                Debug.LogError("Error login: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator GuardarLogroCorrutina(string userId, string nombreLogro)
    {
        string url = supabaseUrl + "/rest/v1/logros_jugador";

        LogroData logro = new LogroData { user_id = userId, nombre_logro = nombreLogro };
        string jsonBody = JsonUtility.ToJson(logro);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", supabaseApiKey);
            request.SetRequestHeader("Authorization", "Bearer " + supabaseApiKey);
            request.SetRequestHeader("Prefer", "return=minimal");

            yield return request.SendWebRequest();

            if (!request.isNetworkError && !request.isHttpError)
            {
                Debug.Log($"¡Logro '{nombreLogro}' guardado con éxito en Supabase!");
            }
            else
            {
                Debug.LogError("Error al guardar el logro: " + request.downloadHandler.text);
            }
        }
    }
}
