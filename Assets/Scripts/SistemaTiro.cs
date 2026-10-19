using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class SistemaTiro : MonoBehaviour
{
    [System.Serializable]
    public class Barra
    {
        public GameObject raiz;          // Fondo de la barra
        public RectTransform zonaVerde;  // Zona de acierto
        public RectTransform indicador;  // Línea móvil
        public bool vertical;            // true = Fuerza, false = Precisión
    }

    [Header("Referencias")]
    public GameManager2 gestor;
    public Camera camaraCarrera;
    public Camera camaraTiro;
    public Transform jugador;
    public Transform aro;
    public Transform pelota;
    public TextMeshProUGUI textoIndicacion;

    [Header("Barras UI")]
    public Barra barraFuerza;
    public Barra barraPrecision;
    public float grosorIndicador = 8f;
    [Range(0f, 1f)] public float centroZonaFuerza = 0.7f;
    [Range(0f, 1f)] public float centroZonaPrecision = 0.5f;

    [Header("Dificultad - Tiro Doble")]
    public float anchoZonaDoble = 0.3f;
    public float velocidadDoble = 1.2f;

    [Header("Dificultad - Tiro Triple")]
    public float anchoZonaTriple = 0.18f;
    public float velocidadTriple = 1.8f;

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
        if (barraFuerza.raiz != null) barraFuerza.raiz.SetActive(false);
        if (barraPrecision.raiz != null) barraPrecision.raiz.SetActive(false);
        if (pelota != null) pelota.gameObject.SetActive(false);
        if (camaraTiro != null) camaraTiro.enabled = false;
        Mensaje("");
    }

    public void Empezar()
    {
        StartCoroutine(Tirar());
    }

    IEnumerator Tirar()
    {
        bool triple = gestor.esTriple;
        float ancho = triple ? anchoZonaTriple : anchoZonaDoble;
        float velocidad = triple ? velocidadTriple : velocidadDoble;

        // 1) Posicionar y activar cámara de tiro
        camaraTiro.transform.position = new Vector3(jugador.position.x, alturaOjos, jugador.position.z);
        camaraTiro.transform.LookAt(aro.position);
        camaraTiro.enabled = true;
        if (camaraCarrera != null) camaraCarrera.enabled = false;

        // Limpiar buffer de teclas antes de arrancar las barras
        yield return new WaitForSeconds(0.2f);

        // 2) Barra 1: Fuerza
        Mensaje("FUERZA: Apretá ESPACIO");
        yield return StartCoroutine(UsarBarra(barraFuerza, centroZonaFuerza, ancho, velocidad));
        float fuerza = valorFrenado;

        yield return new WaitForSeconds(0.2f);

        // 3) Barra 2: Precisión
        Mensaje("PRECISIÓN: Apretá ESPACIO");
        yield return StartCoroutine(UsarBarra(barraPrecision, centroZonaPrecision, ancho, velocidad));
        float precision = valorFrenado;

        Mensaje("");

        // 4) Comprobar acierto
        float inicioFuerza = InicioZona(centroZonaFuerza, ancho);
        float inicioPrecision = InicioZona(centroZonaPrecision, ancho);

        bool fuerzaOk = fuerza >= inicioFuerza && fuerza <= (inicioFuerza + ancho);
        bool precisionOk = precision >= inicioPrecision && precision <= (inicioPrecision + ancho);
        bool convertido = fuerzaOk && precisionOk;

        // 5) Definir trayectoria de la pelota
        Vector3 haciaAro = (aro.position - camaraTiro.transform.position);
        haciaAro.y = 0f;
        haciaAro.Normalize();

        Vector3 destino = aro.position;

        if (!fuerzaOk)
        {
            float signo = fuerza < inicioFuerza ? -1f : 1f; // Corto o largo
            destino += haciaAro * signo * distanciaFallo;
        }

        if (!precisionOk)
        {
            float signo = precision < inicioPrecision ? -1f : 1f; // Izquierda o derecha
            destino += camaraTiro.transform.right * signo * distanciaFallo;
        }

        // 6) Animación de tiro en parábola
        Vector3 inicioPelota = camaraTiro.transform.position + camaraTiro.transform.forward * 0.5f + Vector3.down * 0.3f;
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

        // Caída tras el golpe/canasta
        Vector3 posFinal = pelota.position;
        tiempo = 0f;
        while (tiempo < 0.4f)
        {
            tiempo += Time.deltaTime;
            pelota.position = posFinal + Vector3.down * 10f * tiempo * tiempo;
            yield return null;
        }

        Mensaje(convertido ? "¡CANASTA!" : "¡FALLASTE!");
        yield return new WaitForSeconds(1.2f);

        // 7) Restaurar cámara y finalizar
        if (camaraCarrera != null) camaraCarrera.enabled = true;
        OcultarTodo();
        gestor.TerminarIntento(convertido);
    }

    IEnumerator UsarBarra(Barra barra, float centro, float ancho, float velocidad)
    {
        ConfigurarZona(barra, InicioZona(centro, ancho), ancho);
        barra.raiz.SetActive(true);

        float reloj = 0f;
        float t = 0f;

        // Espera a soltar la tecla si venía presionada
        while (Input.GetKey(KeyCode.Space)) yield return null;

        // Bucle de movimiento del indicador
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            reloj += Time.deltaTime;
            t = Mathf.PingPong(reloj * velocidad, 1f);
            MoverIndicador(barra, t);
            yield return null;
        }

        valorFrenado = t;
        yield return new WaitForSeconds(0.4f);
        barra.raiz.SetActive(false);
    }

    float InicioZona(float centro, float ancho)
    {
        return Mathf.Clamp(centro - (ancho / 2f), 0f, 1f - ancho);
    }

    void ConfigurarZona(Barra barra, float inicio, float ancho)
    {
        RectTransform z = barra.zonaVerde;
        if (barra.vertical)
        {
            z.anchorMin = new Vector2(0f, inicio);
            z.anchorMax = new Vector2(1f, inicio + ancho);
        }
        else
        {
            z.anchorMin = new Vector2(inicio, 0f);
            z.anchorMax = new Vector2(inicio + ancho, 1f);
        }
        z.offsetMin = Vector2.zero;
        z.offsetMax = Vector2.zero;
    }

    void MoverIndicador(Barra barra, float t)
    {
        RectTransform i = barra.indicador;
        float m = grosorIndicador / 2f;
        if (barra.vertical)
        {
            i.anchorMin = new Vector2(0f, t);
            i.anchorMax = new Vector2(1f, t);
            i.offsetMin = new Vector2(0f, -m);
            i.offsetMax = new Vector2(0f, m);
        }
        else
        {
            i.anchorMin = new Vector2(t, 0f);
            i.anchorMax = new Vector2(t, 1f);
            i.offsetMin = new Vector2(-m, 0f);
            i.offsetMax = new Vector2(m, 0f);
        }
    }

    void Mensaje(string texto)
    {
        if (textoIndicacion != null)
            textoIndicacion.text = texto;
    }
}
