using UnityEngine;
using UnityEngine.SceneManagement; // Importante para manejar escenas

public class CambioDeEscena : MonoBehaviour
{
    // Método público para que aparezca en el Inspector del Botón
    public void CargarEscena(string nivelbasquet)
    {
        SceneManager.LoadScene(nivelbasquet);
    }
}
