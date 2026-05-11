using UnityEngine;
using UnityEngine.UI;

// Crea automáticamente un Canvas con controles de runtime pensados para el ejecutable.
public class UIBootstrap : MonoBehaviour
{
    private Font fuente;
    private readonly Color colorPanelPrincipal = new Color(0.08f, 0.1f, 0.14f, 0.78f);
    private readonly Color colorPanelResumen = new Color(0.08f, 0.1f, 0.14f, 0.7f);
    private readonly Color colorBoton = new Color(0.86f, 0.88f, 0.9f, 0.94f);
    private readonly Color colorBotonAccion = new Color(0.71f, 0.76f, 0.8f, 0.94f);
    private readonly Color colorTextoOscuro = new Color(0.11f, 0.13f, 0.16f);
    private readonly Color colorTextoClaro = new Color(0.94f, 0.95f, 0.97f);
    private readonly Color colorTextoSecundario = new Color(0.77f, 0.81f, 0.86f);
    private readonly Color colorInput = new Color(0.96f, 0.97f, 0.98f, 0.95f);
    private readonly Color colorInputPlaceholder = new Color(0.46f, 0.5f, 0.56f);
    private readonly Color colorSliderFondo = new Color(0.24f, 0.28f, 0.34f, 0.92f);
    private readonly Color colorSliderActivo = new Color(0.56f, 0.66f, 0.72f, 1f);

    void Start()
    {
        if (FindObjectOfType<SimuladorUI>() != null) return;
        fuente = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // EventSystem
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 1f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel
        var panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform);
        var panelImg = panelGO.AddComponent<Image>();
        panelImg.color = colorPanelPrincipal;
        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0, 1);
        panelRT.anchorMax = new Vector2(0, 1);
        panelRT.pivot = new Vector2(0, 1);
        panelRT.anchoredPosition = new Vector2(22, -22);
        panelRT.sizeDelta = new Vector2(660, 630);

        Text CrearTexto(string nombre, Transform padre, string texto, int tamano, Color color, TextAnchor alineacion)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(padre, false);
            var txt = go.AddComponent<Text>();
            txt.font = fuente;
            txt.text = texto;
            txt.fontSize = tamano;
            txt.color = color;
            txt.alignment = alineacion;
            return txt;
        }

        // Helper de creación de botón
        Button CrearBoton(string nombre, Vector2 pos, string texto)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(panelGO.transform, false);
            var img = go.AddComponent<Image>();
            bool esAccionPrincipal = texto == "Iniciar" || texto == "Lote" || texto == "Aplicar";
            img.color = esAccionPrincipal ? colorBotonAccion : colorBoton;
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(136, 40);
            rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1); rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = pos;
            var btn = go.AddComponent<Button>();
            var txt = CrearTexto("Text", go.transform, texto, 17, colorTextoOscuro, TextAnchor.MiddleCenter);
            var txtrt = txt.GetComponent<RectTransform>(); txtrt.anchorMin = Vector2.zero; txtrt.anchorMax = Vector2.one; txtrt.offsetMin = Vector2.zero; txtrt.offsetMax = Vector2.zero;
            return btn;
        }

        Text CrearLabel(string nombre, Vector2 pos, string texto)
        {
            var txt = CrearTexto(nombre, panelGO.transform, texto, 16, colorTextoClaro, TextAnchor.MiddleLeft);
            var rt = txt.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(580, 24);
            rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = pos;
            return txt;
        }

        Slider CrearSlider(string nombre, Vector2 pos, float min, float max)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(panelGO.transform, false);
            var bg = go.AddComponent<Image>();
            bg.color = colorSliderFondo;
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(230, 22);
            rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = pos;

            var areaGO = new GameObject("Area");
            areaGO.transform.SetParent(go.transform, false);
            var areaRT = areaGO.AddComponent<RectTransform>();
            areaRT.anchorMin = Vector2.zero;
            areaRT.anchorMax = Vector2.one;
            areaRT.offsetMin = new Vector2(12f, 0f);
            areaRT.offsetMax = new Vector2(-12f, 0f);

            var fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(areaGO.transform, false);
            var fill = fillGO.AddComponent<Image>(); fill.color = colorSliderActivo;
            var fillRT = fillGO.GetComponent<RectTransform>();
            fillRT.anchorMin = new Vector2(0f, 0.25f);
            fillRT.anchorMax = new Vector2(1f, 0.75f);
            fillRT.offsetMin = Vector2.zero;
            fillRT.offsetMax = Vector2.zero;

            var handleGO = new GameObject("Handle");
            handleGO.transform.SetParent(areaGO.transform, false);
            var handle = handleGO.AddComponent<Image>(); handle.color = new Color(0.97f, 0.98f, 0.99f);
            var handleRT = handleGO.GetComponent<RectTransform>(); handleRT.sizeDelta = new Vector2(14, 20);

            var slider = go.AddComponent<Slider>();
            slider.fillRect = fillRT;
            slider.targetGraphic = handle;
            slider.handleRect = handleRT;
            slider.minValue = min; slider.maxValue = max;
            slider.value = (min+max)/2f;
            return slider;
        }

        InputField CrearCampoEntrada(string nombre, Vector2 pos, string placeholder)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(panelGO.transform, false);
            var img = go.AddComponent<Image>();
            img.color = colorInput;
            var campo = go.AddComponent<InputField>();
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(108, 36);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = pos;

            var texto = CrearTexto("Texto", go.transform, string.Empty, 15, colorTextoOscuro, TextAnchor.MiddleLeft);
            var textoRT = texto.GetComponent<RectTransform>();
            textoRT.anchorMin = Vector2.zero;
            textoRT.anchorMax = Vector2.one;
            textoRT.offsetMin = new Vector2(10f, 6f);
            textoRT.offsetMax = new Vector2(-10f, -7f);

            var placeholderTxt = CrearTexto("Placeholder", go.transform, placeholder, 14, colorInputPlaceholder, TextAnchor.MiddleLeft);
            var placeholderRT = placeholderTxt.GetComponent<RectTransform>();
            placeholderRT.anchorMin = Vector2.zero;
            placeholderRT.anchorMax = Vector2.one;
            placeholderRT.offsetMin = new Vector2(10f, 6f);
            placeholderRT.offsetMax = new Vector2(-10f, -7f);
            placeholderTxt.fontStyle = FontStyle.Italic;

            campo.textComponent = texto;
            campo.placeholder = placeholderTxt;
            return campo;
        }

        var titulo = CrearTexto("Titulo", panelGO.transform, "Control de simulación", 24, colorTextoClaro, TextAnchor.MiddleLeft);
        var tituloRT = titulo.GetComponent<RectTransform>();
        tituloRT.anchorMin = new Vector2(0, 1);
        tituloRT.anchorMax = new Vector2(0, 1);
        tituloRT.pivot = new Vector2(0, 1);
        tituloRT.anchoredPosition = new Vector2(22, -18);
        tituloRT.sizeDelta = new Vector2(460, 30);

        var subtitulo = CrearTexto("Subtitulo", panelGO.transform, "Parámetros de simulación y exportación", 15, colorTextoSecundario, TextAnchor.MiddleLeft);
        var subtituloRT = subtitulo.GetComponent<RectTransform>();
        subtituloRT.anchorMin = new Vector2(0, 1);
        subtituloRT.anchorMax = new Vector2(0, 1);
        subtituloRT.pivot = new Vector2(0, 1);
        subtituloRT.anchoredPosition = new Vector2(22, -48);
        subtituloRT.sizeDelta = new Vector2(430, 22);

        // Crear UI elements
        var lblResumen = CrearLabel("lblResumen", new Vector2(22, -82), "Activos: 0 | Infectados: 0 | Contagio: 0%");

        var lblAforo = CrearLabel("lblAforo", new Vector2(22, -126), "Aforo objetivo: 30");
        var inpAforo = CrearCampoEntrada("inpAforo", new Vector2(190, -122), "30");
        inpAforo.contentType = InputField.ContentType.IntegerNumber;
        var sldAforo = CrearSlider("sldAforo", new Vector2(312, -123), 1, 500);

        var lblIntervalo = CrearLabel("lblIntervalo", new Vector2(22, -176), "Intervalo spawn: 2.0s");
        var inpIntervalo = CrearCampoEntrada("inpIntervalo", new Vector2(190, -172), "2.0");
        inpIntervalo.contentType = InputField.ContentType.DecimalNumber;
        var sldIntervalo = CrearSlider("sldIntervalo", new Vector2(312, -173), 0.1f, 10f);

        var lblLote = CrearLabel("lblLote", new Vector2(22, -226), "Lote: 5 simulaciones | 45 s por corrida");
        var inpCantidadSimulaciones = CrearCampoEntrada("inpCantidadSimulaciones", new Vector2(22, -258), "5");
        inpCantidadSimulaciones.contentType = InputField.ContentType.IntegerNumber;
        var inpDuracion = CrearCampoEntrada("inpDuracionSimulacion", new Vector2(144, -258), "45");
        inpDuracion.contentType = InputField.ContentType.DecimalNumber;

        var lblAforoLote = CrearLabel("lblAforoLote", new Vector2(22, -304), "Aforo por corrida: 20 a 60");
        var inpAforoMinimo = CrearCampoEntrada("inpAforoMinimoLote", new Vector2(22, -336), "20");
        inpAforoMinimo.contentType = InputField.ContentType.IntegerNumber;
        var inpAforoMaximo = CrearCampoEntrada("inpAforoMaximoLote", new Vector2(144, -336), "60");
        inpAforoMaximo.contentType = InputField.ContentType.IntegerNumber;

        var lblRuta = CrearLabel("lblRutaSalida", new Vector2(22, -384), "Salida JSON: ");
        var inpRuta = CrearCampoEntrada("inpRutaSalida", new Vector2(22, -418), "C:/MisReportes");
        inpRuta.GetComponent<RectTransform>().sizeDelta = new Vector2(580, 38);

        var btnAplicar = CrearBoton("btnAplicar", new Vector2(22, -470), "Aplicar");
        var btnRutaPortable = CrearBoton("btnRutaPortable", new Vector2(168, -470), "Ruta portable");
        var btnAbrirCarpeta = CrearBoton("btnAbrirCarpeta", new Vector2(314, -470), "Abrir carpeta");
        btnAbrirCarpeta.GetComponent<RectTransform>().sizeDelta = new Vector2(162, 40);

        var btnIniciar = CrearBoton("btnIniciar", new Vector2(22, -522), "Iniciar");
        var btnLote = CrearBoton("btnEjecutarLote", new Vector2(168, -522), "Lote");
        var btnDetener = CrearBoton("btnDetener", new Vector2(314, -522), "Detener");
        var btnReset = CrearBoton("btnReset", new Vector2(460, -522), "Reset");
        var btnExportar = CrearBoton("btnExportar", new Vector2(22, -570), "Exportar actual");
        btnExportar.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 40);

        var lblEstado = CrearLabel("lblEstado", new Vector2(206, -578), "Listo para ejecutar.");
        lblEstado.color = colorTextoSecundario;
        lblEstado.GetComponent<RectTransform>().sizeDelta = new Vector2(390, 24);

        var panelResumenGO = new GameObject("PanelResumen");
        panelResumenGO.transform.SetParent(canvasGO.transform, false);
        var panelResumenImg = panelResumenGO.AddComponent<Image>();
        panelResumenImg.color = colorPanelResumen;
        var panelResumenRT = panelResumenGO.GetComponent<RectTransform>();
        panelResumenRT.anchorMin = new Vector2(1f, 1f);
        panelResumenRT.anchorMax = new Vector2(1f, 1f);
        panelResumenRT.pivot = new Vector2(1f, 1f);
        panelResumenRT.anchoredPosition = new Vector2(-22f, -22f);
        panelResumenRT.sizeDelta = new Vector2(430f, 210f);

        var lblTiempo = CrearTexto("lblTiempo", panelResumenGO.transform, "Tiempo: 00:00", 19, colorTextoClaro, TextAnchor.MiddleLeft);
        var lblTiempoRT = lblTiempo.GetComponent<RectTransform>();
        lblTiempoRT.anchorMin = new Vector2(0, 1);
        lblTiempoRT.anchorMax = new Vector2(0, 1);
        lblTiempoRT.pivot = new Vector2(0, 1);
        lblTiempoRT.anchoredPosition = new Vector2(20f, -20f);
        lblTiempoRT.sizeDelta = new Vector2(270f, 26f);

        var lblCasos = CrearTexto("lblCasos", panelResumenGO.transform, "Iniciales: 0 | Secundarios: 0", 17, colorTextoClaro, TextAnchor.MiddleLeft);
        var lblCasosRT = lblCasos.GetComponent<RectTransform>();
        lblCasosRT.anchorMin = new Vector2(0, 1);
        lblCasosRT.anchorMax = new Vector2(0, 1);
        lblCasosRT.pivot = new Vector2(0, 1);
        lblCasosRT.anchoredPosition = new Vector2(20f, -56f);
        lblCasosRT.sizeDelta = new Vector2(360f, 24f);

        var lblResumenFinal = CrearTexto("lblResumenFinal", panelResumenGO.transform, "Sin resultados todavía.", 16, colorTextoSecundario, TextAnchor.UpperLeft);
        var lblResumenFinalRT = lblResumenFinal.GetComponent<RectTransform>();
        lblResumenFinalRT.anchorMin = new Vector2(0, 1);
        lblResumenFinalRT.anchorMax = new Vector2(0, 1);
        lblResumenFinalRT.pivot = new Vector2(0, 1);
        lblResumenFinalRT.anchoredPosition = new Vector2(20f, -96f);
        lblResumenFinalRT.sizeDelta = new Vector2(388f, 96f);
        lblResumenFinal.horizontalOverflow = HorizontalWrapMode.Wrap;
        lblResumenFinal.verticalOverflow = VerticalWrapMode.Overflow;

        // SimuladorUI
        var ui = panelGO.AddComponent<SimuladorUI>();
        ui.btnIniciar = btnIniciar;
        ui.btnEjecutarLote = btnLote;
        ui.btnDetener = btnDetener;
        ui.btnReset = btnReset;
        ui.btnExportar = btnExportar;
        ui.btnAplicarParametros = btnAplicar;
        ui.btnRutaPortable = btnRutaPortable;
        ui.btnAbrirCarpeta = btnAbrirCarpeta;
        ui.sldAforo = sldAforo;
        ui.sldIntervalo = sldIntervalo;
        ui.inpAforo = inpAforo;
        ui.inpIntervalo = inpIntervalo;
        ui.inpCantidadSimulaciones = inpCantidadSimulaciones;
        ui.inpDuracionSimulacion = inpDuracion;
        ui.inpAforoMinimoLote = inpAforoMinimo;
        ui.inpAforoMaximoLote = inpAforoMaximo;
        ui.inpRutaSalida = inpRuta;
        ui.lblAforo = lblAforo;
        ui.lblIntervalo = lblIntervalo;
        ui.lblLote = lblLote;
        ui.lblAforoLote = lblAforoLote;
        ui.lblRutaSalida = lblRuta;
        ui.lblResumen = lblResumen;
        ui.lblEstado = lblEstado;
        ui.lblTiempo = lblTiempo;
        ui.lblCasos = lblCasos;
        ui.lblResumenFinal = lblResumenFinal;
    }
}
