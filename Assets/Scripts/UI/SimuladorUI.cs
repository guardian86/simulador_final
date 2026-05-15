using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SimuladorUI : MonoBehaviour
{
    private struct LeyendaItem
    {
        public readonly Color Color;
        public readonly string Texto;
        public readonly bool IconoCircular;

        public LeyendaItem(Color color, string texto, bool iconoCircular)
        {
            Color = color;
            Texto = texto;
            IconoCircular = iconoCircular;
        }
    }

    public Administrador admin;

    [Header("Botones")]
    public Button btnIniciar;
    public Button btnDetener;
    public Button btnReset;
    public Button btnExportar;
    public Button btnEjecutarLote;
    public Button btnAplicarParametros;
    public Button btnRutaPortable;
    public Button btnAbrirCarpeta;

    [Header("Sliders")]
    public Slider sldAforo;
    public Slider sldIntervalo;

    [Header("Sliders opcionales de riesgo")]
    public Slider sldVentilacion;
    public Slider sldEficaciaMascarilla;
    public Slider sldAforoPorcentaje;

    [Header("Campos de texto")]
    public InputField inpAforo;
    public InputField inpIntervalo;
    public InputField inpCantidadSimulaciones;
    public InputField inpDuracionSimulacion;
    public InputField inpAforoMinimoLote;
    public InputField inpAforoMaximoLote;
    public InputField inpRutaSalida;

    [Header("Etiquetas")]
    public Text lblAforo;
    public Text lblIntervalo;
    public Text lblLote;
    public Text lblAforoLote;
    public Text lblRutaSalida;
    public Text lblResumen;
    public Text lblEstado;
    public Text lblTiempo;
    public Text lblCasos;
    public Text lblResumenFinal;

    [Header("Textos opcionales de valores")]
    public Text txtVentilacionValor;
    public Text txtEficaciaMascarillaValor;
    public Text txtAforoPorcentajeValor;

    [Header("Leyendas")]
    public RectTransform contenedorLeyendaResultados;
    public RectTransform contenedorLeyendaAgentes;
    public Color colorLineaSusceptibles = new Color(0.2f, 0.52f, 0.9f);
    public Color colorLineaInfectados = new Color(0.85f, 0.22f, 0.22f);
    public Color colorAgenteSano = new Color(0.21f, 0.7f, 0.36f);
    public Color colorAgenteInfectado = new Color(0.88f, 0.22f, 0.22f);

    private bool sincronizandoUI;
    private Font fuenteLeyenda;
    private Sprite spriteCirculoLeyenda;

    private void Start()
    {
        if (admin == null)
            admin = FindObjectOfType<Administrador>();

        ConfigurarEventos();
        SincronizarUIConAdministrador();
        AsegurarLeyendas();
        ActualizarTextosSlidersOpcionales();
        InvokeRepeating(nameof(ActualizarResumen), 0.5f, 1f);
    }

    private void ConfigurarEventos()
    {
        if (btnIniciar != null) btnIniciar.onClick.AddListener(IniciarSimulacion);
        if (btnDetener != null) btnDetener.onClick.AddListener(DetenerSimulacion);
        if (btnReset != null) btnReset.onClick.AddListener(ResetearSimulacion);
        if (btnExportar != null) btnExportar.onClick.AddListener(ExportarReporte);
        if (btnEjecutarLote != null) btnEjecutarLote.onClick.AddListener(EjecutarLote);
        if (btnAplicarParametros != null) btnAplicarParametros.onClick.AddListener(AplicarParametrosVisuales);
        if (btnRutaPortable != null) btnRutaPortable.onClick.AddListener(UsarRutaPortable);
        if (btnAbrirCarpeta != null) btnAbrirCarpeta.onClick.AddListener(AbrirCarpetaReportes);

        if (sldAforo != null)
        {
            sldAforo.minValue = 1f;
            sldAforo.maxValue = 500f;
            sldAforo.onValueChanged.AddListener(ActualizarAforoDesdeSlider);
        }

        if (sldIntervalo != null)
        {
            sldIntervalo.minValue = 0.1f;
            sldIntervalo.maxValue = 10f;
            sldIntervalo.onValueChanged.AddListener(ActualizarIntervaloDesdeSlider);
        }

        if (sldVentilacion != null)
        {
            sldVentilacion.minValue = 0f;
            sldVentilacion.maxValue = 12f;
            sldVentilacion.wholeNumbers = false;
            sldVentilacion.onValueChanged.AddListener(ActualizarVentilacionDesdeSlider);
        }

        if (sldEficaciaMascarilla != null)
        {
            sldEficaciaMascarilla.minValue = 0f;
            sldEficaciaMascarilla.maxValue = 100f;
            sldEficaciaMascarilla.wholeNumbers = true;
            sldEficaciaMascarilla.onValueChanged.AddListener(ActualizarEficaciaMascarillaDesdeSlider);
        }

        if (sldAforoPorcentaje != null)
            sldAforoPorcentaje.onValueChanged.AddListener(_ => ActualizarTextosSlidersOpcionales());

        if (inpAforo != null) inpAforo.onEndEdit.AddListener(_ => AplicarAforoDesdeCampo());
        if (inpIntervalo != null) inpIntervalo.onEndEdit.AddListener(_ => AplicarIntervaloDesdeCampo());
        if (inpCantidadSimulaciones != null) inpCantidadSimulaciones.onEndEdit.AddListener(_ => AplicarCantidadSimulacionesDesdeCampo());
        if (inpDuracionSimulacion != null) inpDuracionSimulacion.onEndEdit.AddListener(_ => AplicarDuracionDesdeCampo());
        if (inpAforoMinimoLote != null) inpAforoMinimoLote.onEndEdit.AddListener(_ => AplicarRangoAforoDesdeCampos());
        if (inpAforoMaximoLote != null) inpAforoMaximoLote.onEndEdit.AddListener(_ => AplicarRangoAforoDesdeCampos());
        if (inpRutaSalida != null) inpRutaSalida.onEndEdit.AddListener(_ => AplicarRutaDesdeCampo());
    }

    private void SincronizarUIConAdministrador()
    {
        if (admin == null)
            return;

        sincronizandoUI = true;

        if (sldAforo != null) sldAforo.value = admin.AforoMaximo;
        if (sldIntervalo != null) sldIntervalo.value = admin.intervaloSpawn;
        if (sldVentilacion != null) sldVentilacion.value = admin.nivelVentilacionACH;
        if (sldEficaciaMascarilla != null) sldEficaciaMascarilla.value = admin.eficaciaMascarillaPorcentaje;

        if (inpAforo != null) inpAforo.text = admin.AforoMaximo.ToString();
        if (inpIntervalo != null) inpIntervalo.text = admin.intervaloSpawn.ToString("0.0", CultureInfo.InvariantCulture);
        if (inpCantidadSimulaciones != null) inpCantidadSimulaciones.text = admin.cantidadSimulacionesLote.ToString();
        if (inpDuracionSimulacion != null) inpDuracionSimulacion.text = admin.duracionSimulacionSegundos.ToString("0.0", CultureInfo.InvariantCulture);
        if (inpAforoMinimoLote != null) inpAforoMinimoLote.text = admin.aforoMinimoPorSimulacion.ToString();
        if (inpAforoMaximoLote != null) inpAforoMaximoLote.text = admin.aforoMaximoPorSimulacion.ToString();
        if (inpRutaSalida != null) inpRutaSalida.text = admin.ObtenerCarpetaSalida();

        sincronizandoUI = false;
        ActualizarEtiquetas();
        ActualizarTextosSlidersOpcionales();
        ActualizarResumen();
    }

    private void ActualizarAforoDesdeSlider(float valor)
    {
        if (sincronizandoUI || admin == null)
            return;

        admin.EstablecerAforoMaximo(Mathf.RoundToInt(valor));
        if (inpAforo != null) inpAforo.text = admin.AforoMaximo.ToString();
        ActualizarEtiquetas();
    }

    private void ActualizarIntervaloDesdeSlider(float valor)
    {
        if (sincronizandoUI || admin == null)
            return;

        admin.EstablecerIntervaloSpawn(valor);
        if (inpIntervalo != null) inpIntervalo.text = admin.intervaloSpawn.ToString("0.0", CultureInfo.InvariantCulture);
        ActualizarEtiquetas();
    }

    private void ActualizarVentilacionDesdeSlider(float valor)
    {
        if (sincronizandoUI || admin == null)
            return;

        admin.EstablecerNivelVentilacion(valor);
        ActualizarTextosSlidersOpcionales();
    }

    private void ActualizarEficaciaMascarillaDesdeSlider(float valor)
    {
        if (sincronizandoUI || admin == null)
            return;

        admin.EstablecerEficaciaMascarilla(valor);
        ActualizarTextosSlidersOpcionales();
    }

    private void AplicarParametrosVisuales()
    {
        AplicarAforoDesdeCampo();
        AplicarIntervaloDesdeCampo();
        AplicarCantidadSimulacionesDesdeCampo();
        AplicarDuracionDesdeCampo();
        AplicarRangoAforoDesdeCampos();
        AplicarRutaDesdeCampo();
        MostrarEstado("Parámetros aplicados.");
    }

    private void AplicarAforoDesdeCampo()
    {
        if (admin == null || inpAforo == null)
            return;

        if (int.TryParse(inpAforo.text, out int aforo))
        {
            admin.EstablecerAforoMaximo(aforo);
            sincronizandoUI = true;
            if (sldAforo != null) sldAforo.value = admin.AforoMaximo;
            sincronizandoUI = false;
            ActualizarEtiquetas();
        }
    }

    private void AplicarIntervaloDesdeCampo()
    {
        if (admin == null || inpIntervalo == null)
            return;

        if (float.TryParse(inpIntervalo.text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float intervalo))
        {
            admin.EstablecerIntervaloSpawn(intervalo);
            sincronizandoUI = true;
            if (sldIntervalo != null) sldIntervalo.value = admin.intervaloSpawn;
            sincronizandoUI = false;
            ActualizarEtiquetas();
        }
    }

    private void AplicarCantidadSimulacionesDesdeCampo()
    {
        if (admin == null || inpCantidadSimulaciones == null)
            return;

        if (int.TryParse(inpCantidadSimulaciones.text, out int cantidad))
        {
            admin.EstablecerCantidadSimulaciones(cantidad);
            ActualizarEtiquetas();
        }
    }

    private void AplicarDuracionDesdeCampo()
    {
        if (admin == null || inpDuracionSimulacion == null)
            return;

        if (float.TryParse(inpDuracionSimulacion.text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out float duracion))
        {
            admin.EstablecerDuracionSimulacion(duracion);
            ActualizarEtiquetas();
        }
    }

    private void AplicarRangoAforoDesdeCampos()
    {
        if (admin == null || inpAforoMinimoLote == null || inpAforoMaximoLote == null)
            return;

        bool okMin = int.TryParse(inpAforoMinimoLote.text, out int minimo);
        bool okMax = int.TryParse(inpAforoMaximoLote.text, out int maximo);
        if (okMin && okMax)
        {
            admin.EstablecerRangoAforoSimulacion(minimo, maximo);
            if (inpAforoMinimoLote != null) inpAforoMinimoLote.text = admin.aforoMinimoPorSimulacion.ToString();
            if (inpAforoMaximoLote != null) inpAforoMaximoLote.text = admin.aforoMaximoPorSimulacion.ToString();
            ActualizarEtiquetas();
        }
    }

    private void AplicarRutaDesdeCampo()
    {
        if (admin == null || inpRutaSalida == null)
            return;

        admin.EstablecerRutaSalida(inpRutaSalida.text);
        ActualizarEtiquetas();
    }

    private void IniciarSimulacion()
    {
        admin?.IniciarSpawn();
        MostrarEstado("Simulación iniciada.");
    }

    private void DetenerSimulacion()
    {
        admin?.DetenerSpawn();
        MostrarEstado("Simulación detenida.");
    }

    private void ResetearSimulacion()
    {
        admin?.ResetSimulacion();
        MostrarEstado("Simulación reiniciada.");
        ActualizarResumen();
    }

    private void ExportarReporte()
    {
        if (admin == null)
            return;

        string ruta = admin.ExportarReporteActual();
        ActualizarResumen();
        MostrarEstado(string.IsNullOrWhiteSpace(ruta) ? "No se pudo exportar el reporte." : $"Reporte exportado en: {ruta}");
    }

    private void EjecutarLote()
    {
        if (admin == null)
            return;

        AplicarParametrosVisuales();
        admin.EjecutarLoteSimulaciones();
        MostrarEstado("Lote de simulaciones iniciado.");
    }

    private void UsarRutaPortable()
    {
        if (admin == null)
            return;

        admin.usarRutaPortable = true;
        admin.EstablecerRutaSalida(string.Empty);
        if (inpRutaSalida != null) inpRutaSalida.text = admin.ObtenerCarpetaSalida();
        ActualizarEtiquetas();
        MostrarEstado("Ruta de escritorio activada.");
    }

    private void AbrirCarpetaReportes()
    {
        if (admin == null)
            return;

        admin.AbrirCarpetaSalida();
        MostrarEstado($"Carpeta abierta: {admin.ObtenerCarpetaSalida()}");
    }

    private void ActualizarEtiquetas()
    {
        if (admin == null)
            return;

        if (lblAforo != null) lblAforo.text = $"Aforo objetivo: {admin.AforoMaximo} personas";
        if (lblIntervalo != null) lblIntervalo.text = $"Intervalo spawn: {admin.intervaloSpawn:0.0}s";
        if (lblLote != null) lblLote.text = $"Lote: {admin.cantidadSimulacionesLote} simulaciones | {admin.duracionSimulacionSegundos:0.#} s por corrida";
        if (lblAforoLote != null) lblAforoLote.text = $"Aforo por corrida: {admin.aforoMinimoPorSimulacion} a {admin.aforoMaximoPorSimulacion} personas";
        if (lblRutaSalida != null) lblRutaSalida.text = $"Salida JSON: {admin.ObtenerCarpetaSalida()}";
    }

    private void ActualizarTextosSlidersOpcionales()
    {
        if (txtVentilacionValor != null && sldVentilacion != null)
            txtVentilacionValor.text = FormatearValorConUnidad(sldVentilacion.value, "ACH");

        if (txtEficaciaMascarillaValor != null && sldEficaciaMascarilla != null)
            txtEficaciaMascarillaValor.text = FormatearValorConUnidad(sldEficaciaMascarilla.value, "%");

        if (txtAforoPorcentajeValor != null && sldAforoPorcentaje != null)
            txtAforoPorcentajeValor.text = FormatearValorConUnidad(sldAforoPorcentaje.value, "%");
    }

    private string FormatearValorConUnidad(float valor, string unidad)
    {
        string numero = Mathf.Abs(valor - Mathf.Round(valor)) < 0.05f
            ? Mathf.RoundToInt(valor).ToString(CultureInfo.InvariantCulture)
            : valor.ToString("0.#", CultureInfo.InvariantCulture);

        return string.IsNullOrWhiteSpace(unidad) ? numero : $"{numero} {unidad}";
    }

    private void ActualizarResumen()
    {
        if (admin == null)
            return;

        int total = admin.ObtenerCantidadAgentesActivos();
        int infectados = admin.ObtenerCantidadAgentesInfectados();
        float porcentaje = total > 0 ? (infectados * 100f) / total : 0f;
        string ventilacion = FormatearValorConUnidad(admin.nivelVentilacionACH, "ACH");
        string mascarilla = FormatearValorConUnidad(admin.eficaciaMascarillaPorcentaje, "%");

        if (lblResumen != null)
            lblResumen.text = $"Contagiados: {porcentaje:0}% | Ventilación: {ventilacion} | Mascarillas: {mascarilla} | I: {infectados}/{total}";

        if (lblTiempo != null)
            lblTiempo.text = $"Tiempo: {FormatearTiempo(admin.TiempoSimulacionActual)}";

        if (lblCasos != null)
            lblCasos.text = $"Iniciales: {admin.ObtenerCantidadCasosIniciales()} | Secundarios: {admin.ObtenerCantidadContagiosSecundariosActuales()}";

        if (lblResumenFinal != null)
            lblResumenFinal.text = admin.ResumenVisualActual;

        if (!string.IsNullOrWhiteSpace(admin.EstadoLoteActual) && lblEstado != null)
            lblEstado.text = admin.EstadoLoteActual;
    }

    private void MostrarEstado(string mensaje)
    {
        if (lblEstado != null)
            lblEstado.text = mensaje;
    }

    // Estas leyendas se generan en runtime para que funcionen tanto en la UI creada por código como en una UI armada en el Canvas.
    private void AsegurarLeyendas()
    {
        var canvasRaiz = ObtenerCanvasRaiz();
        if (canvasRaiz == null)
            return;

        var panelResultados = ObtenerOCrearPanelLeyenda(
            canvasRaiz,
            contenedorLeyendaResultados,
            "LeyendaResultados",
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(-22f, -244f),
            new Vector2(430f, 98f));

        ConstruirLeyenda(
            panelResultados,
            "Leyenda de resultados",
            new[]
            {
                new LeyendaItem(colorLineaSusceptibles, "Susceptibles", false),
                new LeyendaItem(colorLineaInfectados, "Infectados", false)
            });

        var panelAgentes = ObtenerOCrearPanelLeyenda(
            canvasRaiz,
            contenedorLeyendaAgentes,
            "LeyendaAgentes",
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(-22f, 22f),
            new Vector2(280f, 120f));

        ConstruirLeyenda(
            panelAgentes,
            "Leyenda de agentes",
            new[]
            {
                new LeyendaItem(colorAgenteSano, "Agente Sano (S)", true),
                new LeyendaItem(colorAgenteInfectado, "Agente Infectado (I)", true)
            });
    }

    private RectTransform ObtenerCanvasRaiz()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        return canvas != null ? canvas.GetComponent<RectTransform>() : null;
    }

    private RectTransform ObtenerOCrearPanelLeyenda(
        RectTransform canvasRaiz,
        RectTransform contenedorAsignado,
        string nombre,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 sizeDelta)
    {
        if (contenedorAsignado != null)
        {
            AsegurarFondoPanel(contenedorAsignado);
            return contenedorAsignado;
        }

        var existente = canvasRaiz.Find(nombre) as RectTransform;
        if (existente != null)
        {
            AsegurarFondoPanel(existente);
            return existente;
        }

        var panel = new GameObject(nombre, typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        panel.SetParent(canvasRaiz, false);
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
        panel.pivot = pivot;
        panel.anchoredPosition = anchoredPosition;
        panel.sizeDelta = sizeDelta;
        AsegurarFondoPanel(panel);
        return panel;
    }

    private void AsegurarFondoPanel(RectTransform panel)
    {
        var fondo = panel.GetComponent<Image>();
        if (fondo == null)
            fondo = panel.gameObject.AddComponent<Image>();

        if (fondo.color.a <= 0.01f)
            fondo.color = new Color(0.08f, 0.1f, 0.14f, 0.82f);
    }

    private void ConstruirLeyenda(RectTransform panel, string titulo, LeyendaItem[] items)
    {
        var host = panel.Find("RuntimeLegendContent") as RectTransform;
        if (host == null)
        {
            host = new GameObject("RuntimeLegendContent", typeof(RectTransform)).GetComponent<RectTransform>();
            host.SetParent(panel, false);
            host.anchorMin = Vector2.zero;
            host.anchorMax = Vector2.one;
            host.offsetMin = Vector2.zero;
            host.offsetMax = Vector2.zero;
        }

        for (int i = host.childCount - 1; i >= 0; i--)
            Destroy(host.GetChild(i).gameObject);

        CrearTextoLeyenda("Titulo", host, titulo, 16, new Vector2(16f, -14f), new Vector2(240f, 22f));

        int indiceFila = 0;
        for (int i = 0; i < items.Length; i++)
        {
            float posY = -48f - (indiceFila * 28f);
            CrearFilaLeyenda(host, items[i], posY);
            indiceFila++;
        }
    }

    private void CrearFilaLeyenda(RectTransform host, LeyendaItem item, float posY)
    {
        var fila = new GameObject(item.Texto, typeof(RectTransform)).GetComponent<RectTransform>();
        fila.SetParent(host, false);
        fila.anchorMin = new Vector2(0f, 1f);
        fila.anchorMax = new Vector2(0f, 1f);
        fila.pivot = new Vector2(0f, 1f);
        fila.anchoredPosition = new Vector2(16f, posY);
        fila.sizeDelta = new Vector2(260f, 24f);

        var icono = new GameObject("Icono", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        icono.SetParent(fila, false);
        icono.anchorMin = new Vector2(0f, 0.5f);
        icono.anchorMax = new Vector2(0f, 0.5f);
        icono.pivot = new Vector2(0f, 0.5f);
        icono.anchoredPosition = new Vector2(0f, -1f);
        icono.sizeDelta = item.IconoCircular ? new Vector2(16f, 16f) : new Vector2(28f, 6f);

        var imagen = icono.GetComponent<Image>();
        imagen.color = item.Color;
        if (item.IconoCircular)
            imagen.sprite = ObtenerSpriteCircular();

        CrearTextoLeyenda("Texto", fila, item.Texto, 14, new Vector2(32f, -2f), new Vector2(210f, 20f));
    }

    private Text CrearTextoLeyenda(string nombre, RectTransform padre, string texto, int tamano, Vector2 posicion, Vector2 sizeDelta)
    {
        if (fuenteLeyenda == null)
            fuenteLeyenda = Resources.GetBuiltinResource<Font>("Arial.ttf");

        var txt = new GameObject(nombre, typeof(RectTransform), typeof(Text)).GetComponent<Text>();
        txt.transform.SetParent(padre, false);
        txt.font = fuenteLeyenda;
        txt.fontSize = tamano;
        txt.color = new Color(0.94f, 0.95f, 0.97f);
        txt.alignment = TextAnchor.MiddleLeft;
        txt.text = texto;

        var rt = txt.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = posicion;
        rt.sizeDelta = sizeDelta;
        return txt;
    }

    private Sprite ObtenerSpriteCircular()
    {
        if (spriteCirculoLeyenda != null)
            return spriteCirculoLeyenda;

        const int tamano = 32;
        var textura = new Texture2D(tamano, tamano, TextureFormat.ARGB32, false);
        textura.wrapMode = TextureWrapMode.Clamp;
        textura.filterMode = FilterMode.Bilinear;

        var centro = new Vector2((tamano - 1) * 0.5f, (tamano - 1) * 0.5f);
        float radio = tamano * 0.42f;

        for (int y = 0; y < tamano; y++)
        {
            for (int x = 0; x < tamano; x++)
            {
                float distancia = Vector2.Distance(new Vector2(x, y), centro);
                textura.SetPixel(x, y, distancia <= radio ? Color.white : Color.clear);
            }
        }

        textura.Apply();
        spriteCirculoLeyenda = Sprite.Create(textura, new Rect(0f, 0f, tamano, tamano), new Vector2(0.5f, 0.5f));
        return spriteCirculoLeyenda;
    }

    private string FormatearTiempo(float segundos)
    {
        int totalSegundos = Mathf.Max(0, Mathf.FloorToInt(segundos));
        int minutos = totalSegundos / 60;
        int restoSegundos = totalSegundos % 60;
        return $"{minutos:00}:{restoSegundos:00}";
    }
}
