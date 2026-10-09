using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

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
    public string username;
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

[System.Serializable]
public class JugadorEmailResponse
{
    public string email;
}

// Helper para convertir JSON Arrays en Unity
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper != null ? wrapper.Items : null;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}

public class Prueba_Conexion : MonoBehaviour
{
    [Header("Referencia al UIController")]
    public UIController uiController;

    [Header("Credenciales Supabase")]
    public string supabaseUrl = "https://dwnovgbnqydetvjxfcmm.supabase.co";
    public string supabaseApiKey = "sb_publishable_DsiebBzA1MR1wdgT3yzXiQ_HkkYrCqE";

    [Header("UI Registro (Crear Cuenta)")]
    public TMP_InputField registroUsernameInput;
    public TMP_InputField registroNombreInput;
    public TMP_InputField registroApellidoInput;
    public TMP_InputField registroMailInput;
    public TMP_InputField registroPasswordInput;

    [Header("UI Inicio de Sesión")]
    public TMP_InputField loginMailInput; // Podés ingresar mail O username acá
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
        string username = registroUsernameInput != null ? registroUsernameInput.text.Trim() : "";
        string nombre = registroNombreInput != null ? registroNombreInput.text.Trim() : "";
        string apellido = registroApellidoInput != null ? registroApellidoInput.text.Trim() : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Completá mail y contraseña para registrarte.");
            return;
        }

        StartCoroutine(RegistrarUsuarioCorrutina(email, password, username, nombre, apellido));
    }

    public void OnClickIniciarSesion()
    {
        if (loginMailInput == null || loginPasswordInput == null)
        {
            Debug.LogError("Asigná los InputFields de Login en el Inspector de Unity.");
            return;
        }

        string inputUsuario = loginMailInput.text.Trim();
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(inputUsuario) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Completá usuario/mail y contraseña para iniciar sesión.");
            return;
        }

        // Si el texto contiene '@', es un correo. De lo contrario, se busca el email por Username.
        if (inputUsuario.Contains("@"))
        {
            StartCoroutine(IniciarSesionCorrutina(inputUsuario, password));
        }
        else
        {
            StartCoroutine(BuscarEmailYIniciarSesionCorrutina(inputUsuario, password));
        }
    }

    // --- CORRUTINA PARA BUSCAR EMAIL POR USERNAME ---

    IEnumerator BuscarEmailYIniciarSesionCorrutina(string username, string password)
    {
        string url = supabaseUrl + "/rest/v1/jugadores?username=eq." + UnityWebRequest.EscapeURL(username) + "&select=email";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("apikey", supabaseApiKey);
            request.SetRequestHeader("Authorization", "Bearer " + supabaseApiKey);

            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool isError = request.result != UnityWebRequest.Result.Success;
#else
            bool isError = request.isNetworkError || request.isHttpError;
#endif

            if (!isError)
            {
                JugadorEmailResponse[] jugadores = JsonHelper.FromJson<JugadorEmailResponse>(request.downloadHandler.text);

                if (jugadores != null && jugadores.Length > 0 && !string.IsNullOrEmpty(jugadores[0].email))
                {
                    string emailEncontrado = jugadores[0].email;
                    StartCoroutine(IniciarSesionCorrutina(emailEncontrado, password));
                }
                else
                {
                    Debug.LogError("No se encontró ningún usuario con el username: " + username);
                }
            }
            else
            {
                Debug.LogError("Error al consultar el username [" + request.responseCode + "]: " + request.downloadHandler.text);
            }
        }
    }

    // --- CORRUTINAS DE AUTENTICACIÓN ---

    IEnumerator RegistrarUsuarioCorrutina(string email, string password, string username, string nombre, string apellido)
    {
        string url = supabaseUrl + "/auth/v1/signup";

        SignUpRequest body = new SignUpRequest
        {
            email = email,
            password = password,
            data = new UserMetaData { username = username, nombre = nombre, apellido = apellido }
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
                Debug.Log("¡Cuenta creada con éxito!");

                if (uiController != null)
                {
                    uiController.MostrarSeleccionPersonaje();
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

                if (authData != null && authData.user != null)
                {
                    StartCoroutine(GuardarLogroCorrutina(authData.user.id, "Primer Inicio de Sesión", authData.access_token));
                }

                if (uiController != null)
                {
                    uiController.MostrarSeleccionPersonaje();
                }
            }
            else
            {
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