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

    public void OnClickRegistrar()
    {
        // Validar que los campos no estén vacíos antes de enviar a Supabase
        if (string.IsNullOrEmpty(registroMailInput.text) || string.IsNullOrEmpty(registroPasswordInput.text))
        {
            Debug.LogWarning("Completá mail y contraseña para registrarte.");
            return;
        }

        StartCoroutine(RegistrarUsuarioCorrutina(
            registroMailInput.text, 
            registroPasswordInput.text,
            registroNombreInput.text,
            registroApellidoInput.text
        ));
    }

    public void OnClickIniciarSesion()
    {
        if (string.IsNullOrEmpty(loginMailInput.text) || string.IsNullOrEmpty(loginPasswordInput.text))
        {
            Debug.LogWarning("Completá mail y contraseña para iniciar sesión.");
            return;
        }

        StartCoroutine(IniciarSesionCorrutina(loginMailInput.text, loginPasswordInput.text));
    }

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

            if (!request.isNetworkError && !request.isHttpError)
            {
                Debug.Log("¡Cuenta creada con éxito!");
                
                // Transición a la siguiente pantalla tras éxito
                if (uiController != null)
                {
                    uiController.AbrirLogin(); // O directamente la pantalla que corresponda
                }
            }
            else
            {
                Debug.LogError("Error al crear cuenta: " + request.downloadHandler.text);
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
                Debug.Log("¡Inicio de sesión exitoso!");

                // Guardar el logro
                SupabaseAuthResponse authData = JsonUtility.FromJson<SupabaseAuthResponse>(request.downloadHandler.text);
                StartCoroutine(GuardarLogroCorrutina(authData.user.id, "Primer Inicio de Sesión"));

                // Cambiar de pantalla SOLAMENTE si el login fue correcto
                if (uiController != null)
                {
                    // Invocamos el método de UIController para avanzar
                    uiController.SendMessage("MostrarSeleccionPersonaje", SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                Debug.LogError("Error en login: Datos incorrectos o usuario no registrado.");
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
                Debug.Log($"¡Logro '{nombreLogro}' guardado con éxito!");
            }
            else
            {
                Debug.LogError("Error al guardar logro: " + request.downloadHandler.text);
            }
        }
    }
}