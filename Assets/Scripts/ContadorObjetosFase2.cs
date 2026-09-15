using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContadorObjetosFase2 : MonoBehaviour
{
   public float tiempoFase2 = 50f;
    public int minimoParaMejor = 15;
    public int maximoObjetos = 25;

    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoContador;
    public GameManager gameManager;

    private float tiempoRestante;
    private int objetosMovidos;
    private bool faseActiva;

    public void IniciarFase()
    {
        tiempoRestante = tiempoFase2;
        objetosMovidos = 0;
        faseActiva = true;
    }

    void Update()
    {
        if (!faseActiva) return;

        tiempoRestante -= Time.deltaTime;

        if (textoTiempo != null)
        {
            float t = Mathf.Max(0, tiempoRestante);
            textoTiempo.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(t / 60), Mathf.FloorToInt(t % 60));
        }

        if (tiempoRestante <= 0)
        {
            faseActiva = false;
            gameManager.FinalizarFase2(objetosMovidos, minimoParaMejor);
        }
    }

    public void RegistrarObjetoMovido()
    {
        if (!faseActiva) return;

        objetosMovidos++;
        if (textoContador != null)
            textoContador.text = objetosMovidos + " / " + maximoObjetos;

        if (objetosMovidos >= maximoObjetos)
        {
            faseActiva = false;
            gameManager.FinalizarFase2(objetosMovidos, minimoParaMejor);
        }
    }
}
