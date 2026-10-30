using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaTiro : MonoBehaviour
{
    [System.Serializable]
    public class Barra
    {
        public GameObject raiz;          // Contenedor / Fondo de la barra
        public RectTransform zonaVerde;  // Zona verde diseñada en Canvas
        public RectTransform indicador;  // Línea blanca móvil
        public bool vertical;            // TRUE = Fuerza (Vertical), FALSE = Precisión (Horizontal)
    }

    [Header("Referencias")]
    public GameManager2 gestor;
    public Transform jugador;            // FPSController
    public Transform aro;                // Aro de básquet
    public Transform pelota;             // Pelota

    [Header("Barras UI")]
    public Barra barraFuerza;
    public Barra barraPrecision;
    public float velocidadBarra = 1.2f;

    [Header("Pelota y Física")]
    public float alturaOjos = 1.7f;
    public float duracionVuelo = 1.2f;
    public float alturaParabola = 2.5f;
    public float distanciaFallo = 2f;

    private float valorFrenado;

    void Start()
    {
        OcultarTodo();
    }

    public void OcultarTodo()
    {
        // Al arrancar la partida, ambas barras inician desactivadas
        if (barraFuerza.raiz != null) barraFuerza.raiz.SetActive(false);
        if (barraPrecision.raiz != null) barraPrecision.raiz.SetActive(false);
        if (pelota != null) pelota.gameObject.SetActive(false);
    }

    public void Empezar()
    {
        StartCoroutine(Tirar());
    }

    IEnumerator Tirar()
    {

        
        // 1) Orientar la cámara hacia el aro
        Transform camaraTransform = Camera.main.transform;
        camaraTransform.position = new Vector3(jugador.position.x, alturaOjos, jugador.position.z);
        camaraTransform.LookAt(aro.position);

        // Activar AMBAS barras al mismo tiempo al comenzar el tiro
        if (barraFuerza.raiz != null) barraFuerza.raiz.SetActive(true);
        if (barraPrecision.raiz != null) barraPrecision.raiz.SetActive(true);

        // Inicializar posiciones de los indicadores
        MoverIndicadorVertical(barraFuerza, 0f);
        MoverIndicadorHorizontal(barraPrecision, 0.5f);

        yield return new WaitForSeconds(0.2f);

        // 2) Mover Barra Vertical (Fuerza)
        yield return StartCoroutine(UsarBarra(barraFuerza, velocidadBarra));
        float fuerza = valorFrenado;

        yield return new WaitForSeconds(0.2f);

        // 3) Mover Barra Horizontal (Precisión)
        yield return StartCoroutine(UsarBarra(barraPrecision, velocidadBarra));
        float precision = valorFrenado;

        // 4) Comprobar si los indicadores quedaron dentro de sus zonas verdes
        bool fuerzaOk = ComprobarAcierto(barraFuerza, fuerza);
        bool precisionOk = ComprobarAcierto(barraPrecision, precision);
        bool convertido = fuerzaOk && precisionOk;

        // 5) Definir trayectoria de la pelota
        Vector3 haciaAro = (aro.position - camaraTransform.position);
        haciaAro.y = 0f;
        haciaAro.Normalize();

        Vector3 destino = aro.position;

        if (!fuerzaOk)
        {
            destino += haciaAro * (fuerza < 0.5f ? -1f : 1f) * distanciaFallo;
        }

        if (!precisionOk)
        {
            destino += camaraTransform.right * (precision < 0.5f ? -1f : 1f) * distanciaFallo;
        }

        // 6) Animación del tiro
        Vector3 inicioPelota = camaraTransform.position + camaraTransform.forward * 0.5f + Vector3.down * 0.3f;
        pelota.position = inicioPelota;
        pelota.gameObject.SetActive(true);

        float tiempo = 0f;
        while (tiempo < duracionVuelo)
        {
            tiempo += Time.deltaTime;
            float s = Mathf.Clamp01(tiempo / duracionVuelo);
            pelota.position = Vector3.Lerp(inicioPelota, destino, s) + Vector3.up * (4f * alturaParabola * s * (1f - s));
            yield return null;
        }

        // Caída tras el tiro
        Vector3 posFinal = pelota.position;
        tiempo = 0f;
        while (tiempo < 0.4f)
        {
            tiempo += Time.deltaTime;
            pelota.position = posFinal + Vector3.down * 10f * tiempo * tiempo;
            yield return null;
        }

        yield return new WaitForSeconds(1.2f);

        // 7) Finalizar
        OcultarTodo();
        gestor.TerminarIntento(convertido);
    }

    IEnumerator UsarBarra(Barra barra, float velocidad)
    {
        float reloj = 0f;
        float t = 0f;

        while (Input.GetKey(KeyCode.Space)) yield return null;

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            reloj += Time.deltaTime;
            t = Mathf.PingPong(reloj * velocidad, 1f);

            if (barra.vertical == true)
            {
                MoverIndicadorVertical(barra, t);
            }
            else if (barra.vertical == false)
            {
                MoverIndicadorHorizontal(barra, t);
            }

            yield return null;
        }

        valorFrenado = t;
        yield return new WaitForSeconds(0.2f);
    }

    void MoverIndicadorVertical(Barra barra, float t)
    {
        if (barra == null || barra.indicador == null || barra.raiz == null) return;

        RectTransform contenedor = barra.raiz.GetComponent<RectTransform>();
        if (contenedor == null) return;

        float alto = contenedor.rect.height;
        float pos = Mathf.Lerp(-alto / 2f, alto / 2f, t);
        barra.indicador.anchoredPosition = new Vector2(0f, pos);
    }

    void MoverIndicadorHorizontal(Barra barra, float t)
    {
        if (barra == null || barra.indicador == null || barra.raiz == null) return;

        RectTransform contenedor = barra.raiz.GetComponent<RectTransform>();
        if (contenedor == null) return;

        float alto = contenedor.rect.width;
        float pos = Mathf.Lerp(-alto / 2f, alto / 2f, t);
        barra.indicador.anchoredPosition = new Vector2(0f, pos);
    }

    bool ComprobarAcierto(Barra barra, float valorT)
    {
        if (barra == null || barra.zonaVerde == null || barra.raiz == null) return true;

        RectTransform contenedor = barra.raiz.GetComponent<RectTransform>();
        float tamanoContenedor = barra.vertical ? contenedor.rect.height : contenedor.rect.width;
        float posIndicador = Mathf.Lerp(-tamanoContenedor / 2f, tamanoContenedor / 2f, valorT);

        float inicioZona = barra.vertical ? barra.zonaVerde.anchoredPosition.y - (barra.zonaVerde.rect.height / 2f) 
                                          : barra.zonaVerde.anchoredPosition.x - (barra.zonaVerde.rect.width / 2f);
        float finZona = inicioZona + (barra.vertical ? barra.zonaVerde.rect.height : barra.zonaVerde.rect.width);

        return posIndicador >= inicioZona && posIndicador <= finZona;
    }
}