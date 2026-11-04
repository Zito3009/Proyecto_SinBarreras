using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SistemaTiro : MonoBehaviour
{
    [System.Serializable]
    public class Barra
    {
        public RectTransform raiz;        // Cambiado a RectTransform para evitar GetComponent en cada frame
        public RectTransform zonaVerde;  // Zona verde diseñada en Canvas
        public RectTransform indicador;  // Línea blanca móvil
        public bool vertical;            // TRUE = Vertical (Eje Y), FALSE = Horizontal (Eje X)
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

    // Control de estado interno y restauración de cámara
    private bool enTiro = false;
    private float valorFrenado;
    private Vector3 camaraPosOriginal;
    private Quaternion camaraRotOriginal;

    void Start()
    {
        // Forzamos la lógica del eje por código para evitar errores de configuración en el Inspector
        if (barraFuerza != null) barraFuerza.vertical = true;
        if (barraPrecision != null) barraPrecision.vertical = false;

        OcultarTodo();
    }

    public void OcultarTodo()
    {
<<<<<<< Updated upstream
        // Al arrancar la partida, ambas barras inician desactivadas
        if (barraFuerza.raiz != null) barraFuerza.raiz.SetActive(false);
        if (barraPrecision.raiz != null) barraPrecision.raiz.SetActive(false);
=======
        if (barraFuerza != null && barraFuerza.raiz != null) barraFuerza.raiz.gameObject.SetActive(false);
        if (barraPrecision != null && barraPrecision.raiz != null) barraPrecision.raiz.gameObject.SetActive(false);
>>>>>>> Stashed changes
        if (pelota != null) pelota.gameObject.SetActive(false);
    }

    public void Empezar()
    {
        // Bloqueo de seguridad: Evita ejecutar corrutinas dobles si se llama a Empezar() repetidamente
        if (enTiro) return;
        StartCoroutine(Tirar());
    }

    IEnumerator Tirar()
    {
<<<<<<< Updated upstream

        
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
=======
        enTiro = true;

        // Comprobación de nulos al inicio
        Camera camaraPrincipal = Camera.main;
        if (camaraPrincipal == null || jugador == null || aro == null || pelota == null)
        {
            Debug.LogError("[SistemaTiro] Falta asignar referencias en el Inspector o falta la Tag 'MainCamera'.");
            enTiro = false;
            yield break;
        }

        Transform camaraTransform = camaraPrincipal.transform;

        // 1) Guardar posición y rotación original de la cámara antes de orientarla
        camaraPosOriginal = camaraTransform.position;
        camaraRotOriginal = camaraTransform.rotation;

        camaraTransform.position = new Vector3(jugador.position.x, jugador.position.y + alturaOjos, jugador.position.z);
        camaraTransform.LookAt(aro.position);

        // Activar barras en pantalla
        if (barraFuerza.raiz != null) barraFuerza.raiz.gameObject.SetActive(true);
        if (barraPrecision.raiz != null) barraPrecision.raiz.gameObject.SetActive(true);

        MoverIndicador(barraFuerza, 0f);
        MoverIndicador(barraPrecision, 0.5f);

        yield return new WaitForSeconds(0.2f);

        // 2) Mover Barra Vertical (Fuerza) iniciando abajo (0.0)
        Mensaje("FUERZA: Apretá ESPACIO");
        yield return StartCoroutine(UsarBarra(barraFuerza, velocidadBarra, 0f));
>>>>>>> Stashed changes
        float fuerza = valorFrenado;

        yield return new WaitForSeconds(0.2f);

<<<<<<< Updated upstream
        // 3) Mover Barra Horizontal (Precisión)
        yield return StartCoroutine(UsarBarra(barraPrecision, velocidadBarra));
        float precision = valorFrenado;

        // 4) Comprobar si los indicadores quedaron dentro de sus zonas verdes
=======
        // 3) Mover Barra Horizontal (Precisión) iniciando en el centro (0.5) suavemente
        Mensaje("PRECISIÓN: Apretá ESPACIO");
        yield return StartCoroutine(UsarBarra(barraPrecision, velocidadBarra, 0.5f));
        float precision = valorFrenado;

        Mensaje("");

        // 4) Comprobar acierto
>>>>>>> Stashed changes
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

        // 7) Restaurar cámara a la posición y rotación previa
        camaraTransform.position = camaraPosOriginal;
        camaraTransform.rotation = camaraRotOriginal;

        // Ocultar elementos y terminar tiro
        OcultarTodo();
        enTiro = false;

        if (gestor != null)
        {
            gestor.TerminarIntento(convertido);
        }
    }

    IEnumerator UsarBarra(Barra barra, float velocidad, float valorInicial = 0f)
    {
        // Se inicializa el reloj en concordancia con el valor inicial de 't'
        float reloj = valorInicial / velocidad;
        float t = valorInicial;

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

<<<<<<< Updated upstream
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
=======
    void MoverIndicador(Barra barra, float t)
    {
        if (barra == null || barra.indicador == null || barra.raiz == null) return;

        // Se utiliza 'barra.vertical' directamente en lugar de comparar nombres
        if (barra.vertical)
        {
            // VERTICAL: Considera el tamaño de la aguja para que no sobresalga del marco
            float altoContenedor = barra.raiz.rect.height;
            float altoIndicador = barra.indicador.rect.height;
            float limite = (altoContenedor - altoIndicador) / 2f;

            float pos = Mathf.Lerp(-limite, limite, t);
            barra.indicador.anchoredPosition = new Vector2(0f, pos);
        }
        else
        {
            // HORIZONTAL: Considera el tamaño de la aguja en el eje X
            float anchoContenedor = barra.raiz.rect.width;
            float anchoIndicador = barra.indicador.rect.width;
            float limite = (anchoContenedor - anchoIndicador) / 2f;

            float pos = Mathf.Lerp(-limite, limite, t);
            barra.indicador.anchoredPosition = new Vector2(pos, 0f);
        }
    }
>>>>>>> Stashed changes

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