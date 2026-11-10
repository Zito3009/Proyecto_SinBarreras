using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaTiro : MonoBehaviour
{
    [System.Serializable]
    public class Barra
    {
        public RectTransform raiz;        // Contenedor principal de la barra
        public RectTransform zonaVerde;   // Zona de acierto
        public RectTransform indicador;   // Línea/aguja móvil
        public bool vertical;             // TRUE = Vertical (Eje Y), FALSE = Horizontal (Eje X)
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

    // Control de estado interno
    private bool enTiro = false;
    private float valorFrenado;
    private Vector3 camaraPosOriginal;
    private Quaternion camaraRotOriginal;

    void Start()
    {
        // Fuerza la orientación según el tipo de barra
        if (barraFuerza != null) barraFuerza.vertical = true;
        if (barraPrecision != null) barraPrecision.vertical = false;

        OcultarTodo();
    }

    public void OcultarTodo()
    {
        if (barraFuerza != null && barraFuerza.raiz != null) barraFuerza.raiz.gameObject.SetActive(false);
        if (barraPrecision != null && barraPrecision.raiz != null) barraPrecision.raiz.gameObject.SetActive(false);
        if (pelota != null) pelota.gameObject.SetActive(false);
    }

    public void Empezar()
    {
        if (enTiro) return;
        StartCoroutine(Tirar());
    }

    IEnumerator Tirar()
    {
        enTiro = true;

        Camera camaraPrincipal = Camera.main;
        if (camaraPrincipal == null || jugador == null || aro == null || pelota == null)
        {
            Debug.LogError("[SistemaTiro] Falta asignar referencias en el Inspector o falta la Tag 'MainCamera'.");
            enTiro = false;
            yield break;
        }

        Transform camaraTransform = camaraPrincipal.transform;

        // Guardar transformación original de la cámara
        camaraPosOriginal = camaraTransform.position;
        camaraRotOriginal = camaraTransform.rotation;

        // Posicionar cámara apuntando al aro
        camaraTransform.position = new Vector3(jugador.position.x, jugador.position.y + alturaOjos, jugador.position.z);
        camaraTransform.LookAt(aro.position);

        // Activar barras en interfaz
        if (barraFuerza.raiz != null) barraFuerza.raiz.gameObject.SetActive(true);
        if (barraPrecision.raiz != null) barraPrecision.raiz.gameObject.SetActive(true);

        MoverIndicador(barraFuerza, 0f);
        MoverIndicador(barraPrecision, 0.5f);

        yield return new WaitForSeconds(0.2f);

        // 1) Barra de Fuerza (Vertical)
        yield return StartCoroutine(UsarBarra(barraFuerza, velocidadBarra, 0f));
        float fuerza = valorFrenado;

        yield return new WaitForSeconds(0.2f);

        // 2) Barra de Precisión (Horizontal)
        yield return StartCoroutine(UsarBarra(barraPrecision, velocidadBarra, 0.5f));
        float precision = valorFrenado;

        // Comprobar aciertos
        bool fuerzaOk = ComprobarAcierto(barraFuerza, fuerza);
        bool precisionOk = ComprobarAcierto(barraPrecision, precision);
        bool convertido = fuerzaOk && precisionOk;

        // Calcular destino final
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

        // Animación de la parábola
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

        // Caída libre tras tiro
        Vector3 posFinal = pelota.position;
        tiempo = 0f;
        while (tiempo < 0.4f)
        {
            tiempo += Time.deltaTime;
            pelota.position = posFinal + Vector3.down * 10f * tiempo * tiempo;
            yield return null;
        }

        yield return new WaitForSeconds(1.2f);

        // Restaurar posición de cámara original
        camaraTransform.position = camaraPosOriginal;
        camaraTransform.rotation = camaraRotOriginal;

        OcultarTodo();
        enTiro = false;

        if (gestor != null)
        {
            gestor.TerminarIntento(convertido);
        }
    }

    IEnumerator UsarBarra(Barra barra, float velocidad, float valorInicial = 0f)
    {
        float reloj = valorInicial / velocidad;
        float t = valorInicial;

        // Espera a que el jugador suelte espacio antes de empezar a medir la nueva tecla
        while (Input.GetKey(KeyCode.Space)) yield return null;

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            reloj += Time.deltaTime;
            t = Mathf.PingPong(reloj * velocidad, 1f);

            MoverIndicador(barra, t);

            yield return null;
        }

        valorFrenado = t;
        yield return new WaitForSeconds(0.2f);
    }

    void MoverIndicador(Barra barra, float t)
    {
        if (barra == null || barra.indicador == null || barra.raiz == null) return;

        if (barra.vertical)
        {
            float altoContenedor = barra.raiz.rect.height;
            float altoIndicador = barra.indicador.rect.height;
            float limite = (altoContenedor - altoIndicador) / 2f;

            float pos = Mathf.Lerp(-limite, limite, t);
            barra.indicador.anchoredPosition = new Vector2(0f, pos);
        }
        else
        {
            float anchoContenedor = barra.raiz.rect.width;
            float anchoIndicador = barra.indicador.rect.width;
            float limite = (anchoContenedor - anchoIndicador) / 2f;

            float pos = Mathf.Lerp(-limite, limite, t);
            barra.indicador.anchoredPosition = new Vector2(pos, 0f);
        }
    }

    bool ComprobarAcierto(Barra barra, float valorT)
    {
        if (barra == null || barra.zonaVerde == null || barra.raiz == null) return true;

        float tamanoContenedor = barra.vertical ? barra.raiz.rect.height : barra.raiz.rect.width;
        float posIndicador = Mathf.Lerp(-tamanoContenedor / 2f, tamanoContenedor / 2f, valorT);

        float inicioZona = barra.vertical 
            ? barra.zonaVerde.anchoredPosition.y - (barra.zonaVerde.rect.height / 2f) 
            : barra.zonaVerde.anchoredPosition.x - (barra.zonaVerde.rect.width / 2f);

        float finZona = inicioZona + (barra.vertical ? barra.zonaVerde.rect.height : barra.zonaVerde.rect.width);

        return posIndicador >= inicioZona && posIndicador <= finZona;
    }
}