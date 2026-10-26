using System.Collections;
using UnityEngine;

public class Transicion_niveles : MonoBehaviour
{
    [Header("Configuración")]
    public CanvasGroup canvasGroup;
    public float duracionFundido = 2.0f; // Tiempo en segundos para el efecto

    void Start()
    {
        // Al iniciar la escena, aseguramos que empiece invisible
        OcultarInstantaneo();
    }

    // Pone la pantalla negra progresivamente
    public void AparecerDeAPoco()
    {
        StopAllCoroutines();
        StartCoroutine(RutinaFundidoIn());
    }

    // Quita la pantalla negra progresivamente (Aclarar)
    public void DesaparecerDeAPoco()
    {
        StopAllCoroutines();
        StartCoroutine(RutinaFundidoOut());
    }

    // Quita la pantalla negra de golpe
    public void OcultarInstantaneo()
    {
        StopAllCoroutines();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator RutinaFundidoIn()
    {
        float tiempo = 0f;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }

        while (tiempo < duracionFundido)
        {
            tiempo += Time.unscaledDeltaTime;
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Clamp01(tiempo / duracionFundido);
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private IEnumerator RutinaFundidoOut()
    {
        float tiempo = 0f;
        float alphaInicial = canvasGroup != null ? canvasGroup.alpha : 1f;

        while (tiempo < duracionFundido)
        {
            tiempo += Time.unscaledDeltaTime;
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Clamp01(Mathf.Lerp(alphaInicial, 0f, tiempo / duracionFundido));
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}