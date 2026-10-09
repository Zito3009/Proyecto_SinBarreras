using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

// --- ESTRUCTURAS DE DATOS (JSON) ---

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
    public string email;
}

[System.Serializable]
public class SupabaseAuthResponse
{
    public string access_token;
    public SupabaseUser user;
}

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

[System.Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}

public class Prueba_Conexion : MonoBehaviour
{
    [Header("Referencia al UIController")]
    public UIController uiController; // Arrastrá el GameObject con UIController acá

    [Header("Credenciales Supabase")]
    public string supabaseUrl = "https://dwnovgbnqydetvjxfcmm.supabase.co";
    public string supabaseApiKey = "sb_publishable_DsiebBzA1MR1wdgT3yzXiQ_HkkYrCqE";

    [Header("UI Registro (Crear Cuenta)")]
    public TMP_InputField registroNombreInput;
    public TMP_InputField registroApellidoInput;
    public TMP_InputField registroMailInput;
    public TMP_InputField registroPasswordInput;

    [Header("UI Inicio de Sesión")]
    public TMP_InputField loginMailInput;
    public TMP_InputField loginPasswordInput;

    // --- MÉTODOS PÚBLICOS PARA LOS BOTONES ---

    public void OnClickRegistrar()
    {
        if (registroMailInput == null || registroPasswordInput == null)
        {
            Debug.LogError("Asigná los InputFields de Registro en el Inspector de Unity.");
            return;
        }

        string email = registroMailInput.text.Trim();
        string password = registroPasswordInput.text;
        string nombre = registroNombreInput != null ? registroNombreInput.text.Trim() : "";
        string apellido = registroApellidoInput != null ? registroApellidoInput.text.Trim() : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Completá mail y contraseña para registrarte.");
            return;
        }

        StartCoroutine(RegistrarUsuarioCorrutina(email, password, nombre, apellido));
    }

    public void OnClickIniciarSesion()
    {
        if (loginMailInput == null || loginPasswordInput == null)
        {
            Debug.LogError("Asigná los InputFields de Login en el Inspector de Unity.");
            return;
        }

        string email = loginMailInput.text.Trim();
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Completá mail y contraseña para iniciar sesión.");
            return;
        }

        StartCoroutine(IniciarSesionCorrutina(email, password));
    }

    // --- CORRUTINAS DE AUTENTICACIÓN ---

    IEnumerator RegistrarUsuarioCorrutina(string email, string password, string nombre, string apellido)
    {
        string url = supabaseUrl + "/auth/v1/signup";

        SignUpRequest body = new SignUpRequest
        {
            email = email,
            password = password,
            data = new UserMetaData { nombre = nombre, apellido = apellido }
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

#if UNITY_2020_1_OR_NEWER
            bool isError = request.result != UnityWebRequest.Result.Success;
#else
            bool isError = request.isNetworkError || request.isHttpError;
#endif

            if (!isError)
            {
                Debug.Log("¡Cuenta creada con éxito! Revisá tu casilla de correo para confirmarla.");

                // Redirigir a la pantalla de Login tras registrarse
                if (uiController != null)
                {
                    uiController.AbrirLogin();
                }
            }
            else
            {
                Debug.LogError("Error al crear cuenta [" + request.responseCode + "]: " + request.downloadHandler.text);
            }
        }
    }

    IEnumerator IniciarSesionCorrutina(string email, string password)
    {
        string url = supabaseUrl + "/auth/v1/token?grant_type=password";

        LoginRequest body = new LoginRequest
        {
            email = email,
            password = password
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

#if UNITY_2020_1_OR_NEWER
            bool isError = request.result != UnityWebRequest.Result.Success;
#else
            bool isError = request.isNetworkError || request.isHttpError;
#endif

            if (!isError)
            {
                Debug.Log("¡Inicio de sesión exitoso!");

                SupabaseAuthResponse authData = JsonUtility.FromJson<SupabaseAuthResponse>(request.downloadHandler.text);

                // Guardar logro inicial del usuario
                if (authData != null && authData.user != null)
                {
                    StartCoroutine(GuardarLogroCorrutina(authData.user.id, "Primer Inicio de Sesión", authData.access_token));
                }

                // Transición a la pantalla de Selección de Personaje
                if (uiController != null)
                {
                    uiController.MostrarSeleccionPersonaje();
                }
            }
            else
            {
                // Muestra el motivo exacto enviado por Supabase en la Consola
                Debug.LogError("Error en login [" + request.responseCode + "]: " + request.downloadHandler.text);
            }
        }
    }

    // --- CORRUTINA DE LOGROS ---

    IEnumerator GuardarLogroCorrutina(string userId, string nombreLogro, string userToken)
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

            // Usar el token JWT del usuario para autenticar la inserción en la base de datos
            string bearerToken = !string.IsNullOrEmpty(userToken) ? userToken : supabaseApiKey;
            request.SetRequestHeader("Authorization", "Bearer " + bearerToken);
            request.SetRequestHeader("Prefer", "return=minimal");

            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool isError = request.result != UnityWebRequest.Result.Success;
#else
            bool isError = request.isNetworkError || request.isHttpError;
#endif

            if (!isError)
            {
                Debug.Log($"¡Logro '{nombreLogro}' guardado con éxito!");
            }
            else
            {
                Debug.LogError("Error al guardar logro [" + request.responseCode + "]: " + request.downloadHandler.text);
            }
        }
    }
}