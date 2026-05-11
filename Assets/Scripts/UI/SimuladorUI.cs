using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class SimuladorUI : MonoBehaviour
{
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

    private bool sincronizandoUI;

    private void Start()
    {
        if (admin == null)
            admin = FindObjectOfType<Administrador>();

        ConfigurarEventos();
        SincronizarUIConAdministrador();
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

        if (inpAforo != null) inpAforo.text = admin.AforoMaximo.ToString();
        if (inpIntervalo != null) inpIntervalo.text = admin.intervaloSpawn.ToString("0.0", CultureInfo.InvariantCulture);
    if (inpCantidadSimulaciones != null) inpCantidadSimulaciones.text = admin.cantidadSimulacionesLote.ToString();
    if (inpDuracionSimulacion != null) inpDuracionSimulacion.text = admin.duracionSimulacionSegundos.ToString("0.0", CultureInfo.InvariantCulture);
    if (inpAforoMinimoLote != null) inpAforoMinimoLote.text = admin.aforoMinimoPorSimulacion.ToString();
    if (inpAforoMaximoLote != null) inpAforoMaximoLote.text = admin.aforoMaximoPorSimulacion.ToString();
        if (inpRutaSalida != null) inpRutaSalida.text = admin.ObtenerCarpetaSalida();

        sincronizandoUI = false;
        ActualizarEtiquetas();
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
        MostrarEstado("Ruta portable activada.");
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

        if (lblAforo != null) lblAforo.text = $"Aforo objetivo: {admin.AforoMaximo}";
        if (lblIntervalo != null) lblIntervalo.text = $"Intervalo spawn: {admin.intervaloSpawn:0.0}s";
        if (lblLote != null) lblLote.text = $"Lote: {admin.cantidadSimulacionesLote} simulaciones | {admin.duracionSimulacionSegundos:0.#} s por corrida";
        if (lblAforoLote != null) lblAforoLote.text = $"Aforo por corrida: {admin.aforoMinimoPorSimulacion} a {admin.aforoMaximoPorSimulacion}";
        if (lblRutaSalida != null) lblRutaSalida.text = $"Salida JSON: {admin.ObtenerCarpetaSalida()}";
    }

    private void ActualizarResumen()
    {
        if (admin == null)
            return;

        int total = admin.ObtenerCantidadAgentesActivos();
        int infectados = admin.ObtenerCantidadAgentesInfectados();
        float porcentaje = total > 0 ? (infectados * 100f) / total : 0f;

        if (lblResumen != null)
            lblResumen.text = $"Activos: {total} | Infectados: {infectados} | Contagio: {porcentaje:0}%";

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

    private string FormatearTiempo(float segundos)
    {
        int totalSegundos = Mathf.Max(0, Mathf.FloorToInt(segundos));
        int minutos = totalSegundos / 60;
        int restoSegundos = totalSegundos % 60;
        return $"{minutos:00}:{restoSegundos:00}";
    }
}
