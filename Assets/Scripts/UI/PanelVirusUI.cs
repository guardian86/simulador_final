using UnityEngine;
using UnityEngine.UI;

// Panel "Perfil de virus": permite cambiar en tiempo de ejecución los parámetros del virus
// (emisión de aerosoles, radio de influencia, atenuación ambiental y umbral de contagio).
// Por defecto se carga el perfil de SARS-CoV-2 (COVID-19). Los valores se guardan en el
// Administrador y quedan registrados en el reporte JSON de cada ejecución.
public class PanelVirusUI : MonoBehaviour
{
    public const string NombreCovid = "SARS-CoV-2 (COVID-19)";
    public const string NombrePersonalizado = "Personalizado (base SARS-CoV-2)";
    const float EmisionCovid = 0.45f;
    const float RadioCovid = 2.5f;
    const float AtenuacionCovid = 0.9f;
    // Dosis infecciosa característica calibrada contra el Aula de Lelieveld et al. (2020):
    // 2.4 contagios secundarios esperados en 12 h con un caso índice y 25 personas (IC95 829-982).
    const float UmbralCovid = 904f;

    public Slider sldEmision, sldRadio, sldAtenuacion, sldUmbral;
    public Text txtEmision, txtRadio, txtAtenuacion, txtUmbral, txtNombre;
    public Button btnRestablecer;

    private Administrador admin;
    private bool iniciado;

    void Start()
    {
        admin = FindObjectOfType<Administrador>();
        sldEmision.onValueChanged.AddListener(v => { if (admin != null) admin.tasaEmisionAerosol = v; AlCambiar(); });
        sldRadio.onValueChanged.AddListener(v => { if (admin != null) admin.radioInfluenciaAerosol = v; AlCambiar(); });
        sldAtenuacion.onValueChanged.AddListener(v => { if (admin != null) admin.factorAtenuacionAmbiente = v; AlCambiar(); });
        sldUmbral.onValueChanged.AddListener(v => { if (admin != null) admin.umbralContagioAerosolBase = v; AlCambiar(); });
        btnRestablecer.onClick.AddListener(Restablecer);
    }

    void Update()
    {
        if (admin == null)
        {
            admin = FindObjectOfType<Administrador>();
            return;
        }

        // Se lee una vez, después de que el Administrador cargó su configuración en Start().
        if (!iniciado)
        {
            LeerDesdeAdministrador();
            iniciado = true;
        }

        // No se permite cambiar el virus mientras corre un lote, para no mezclar perfiles.
        bool editable = !admin.LoteEnEjecucion;
        sldEmision.interactable = editable;
        sldRadio.interactable = editable;
        sldAtenuacion.interactable = editable;
        sldUmbral.interactable = editable;
        btnRestablecer.interactable = editable;
    }

    void LeerDesdeAdministrador()
    {
        sldEmision.SetValueWithoutNotify(admin.tasaEmisionAerosol);
        sldRadio.SetValueWithoutNotify(admin.radioInfluenciaAerosol);
        sldAtenuacion.SetValueWithoutNotify(admin.factorAtenuacionAmbiente);
        sldUmbral.SetValueWithoutNotify(admin.umbralContagioAerosolBase);
        RefrescarTextos();
    }

    void Restablecer()
    {
        if (admin == null) return;
        admin.nombreVirus = NombreCovid;
        admin.tasaEmisionAerosol = EmisionCovid;
        admin.radioInfluenciaAerosol = RadioCovid;
        admin.factorAtenuacionAmbiente = AtenuacionCovid;
        admin.umbralContagioAerosolBase = UmbralCovid;
        LeerDesdeAdministrador();
    }

    void AlCambiar()
    {
        if (admin == null) return;
        bool esCovid = Mathf.Approximately(admin.tasaEmisionAerosol, EmisionCovid)
            && Mathf.Approximately(admin.radioInfluenciaAerosol, RadioCovid)
            && Mathf.Approximately(admin.factorAtenuacionAmbiente, AtenuacionCovid)
            && Mathf.Approximately(admin.umbralContagioAerosolBase, UmbralCovid);
        admin.nombreVirus = esCovid ? NombreCovid : NombrePersonalizado;
        RefrescarTextos();
    }

    void RefrescarTextos()
    {
        txtEmision.text = admin.tasaEmisionAerosol.ToString("0.00");
        txtRadio.text = admin.radioInfluenciaAerosol.ToString("0.0") + " m";
        txtAtenuacion.text = admin.factorAtenuacionAmbiente.ToString("0.00");
        txtUmbral.text = admin.umbralContagioAerosolBase.ToString("0");
        txtNombre.text = "Virus: " + admin.nombreVirus;
    }

    // Construye el panel (esquina superior derecha, debajo del resumen) y lo conecta.
    public static void Crear(Transform canvas, Font fuente)
    {
        Color colorPanel = new Color(0.08f, 0.1f, 0.14f, 0.78f);
        Color claro = new Color(0.94f, 0.95f, 0.97f);
        Color secundario = new Color(0.77f, 0.81f, 0.86f);
        Color oscuro = new Color(0.11f, 0.13f, 0.16f);
        Color sliderFondo = new Color(0.24f, 0.28f, 0.34f, 0.92f);
        Color sliderActivo = new Color(0.56f, 0.66f, 0.72f, 1f);
        Color boton = new Color(0.86f, 0.88f, 0.9f, 0.94f);

        var panelGO = new GameObject("PanelPerfilVirus");
        panelGO.transform.SetParent(canvas, false);
        panelGO.AddComponent<Image>().color = colorPanel;
        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(1f, 1f);
        panelRT.anchorMax = new Vector2(1f, 1f);
        panelRT.pivot = new Vector2(1f, 1f);
        panelRT.anchoredPosition = new Vector2(-22f, -372f);
        panelRT.sizeDelta = new Vector2(430f, 346f);

        Text Texto(string nombre, Vector2 pos, Vector2 tam, string texto, int tamano, Color color, TextAnchor ancla)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(panelGO.transform, false);
            var t = go.AddComponent<Text>();
            t.font = fuente; t.text = texto; t.fontSize = tamano; t.color = color; t.alignment = ancla;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1); rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = pos; rt.sizeDelta = tam;
            return t;
        }

        Slider Deslizador(string nombre, Vector2 pos, float min, float max)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(panelGO.transform, false);
            go.AddComponent<Image>().color = sliderFondo;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1); rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(386f, 22f);

            var areaGO = new GameObject("Area");
            areaGO.transform.SetParent(go.transform, false);
            var areaRT = areaGO.AddComponent<RectTransform>();
            areaRT.anchorMin = Vector2.zero; areaRT.anchorMax = Vector2.one;
            areaRT.offsetMin = new Vector2(12f, 0f); areaRT.offsetMax = new Vector2(-12f, 0f);

            var fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(areaGO.transform, false);
            fillGO.AddComponent<Image>().color = sliderActivo;
            var fillRT = fillGO.GetComponent<RectTransform>();
            fillRT.anchorMin = new Vector2(0f, 0.25f); fillRT.anchorMax = new Vector2(1f, 0.75f);
            fillRT.offsetMin = Vector2.zero; fillRT.offsetMax = Vector2.zero;

            var handleGO = new GameObject("Handle");
            handleGO.transform.SetParent(areaGO.transform, false);
            var handle = handleGO.AddComponent<Image>(); handle.color = new Color(0.97f, 0.98f, 0.99f);
            var handleRT = handleGO.GetComponent<RectTransform>(); handleRT.sizeDelta = new Vector2(14, 20);

            var s = go.AddComponent<Slider>();
            s.fillRect = fillRT; s.targetGraphic = handle; s.handleRect = handleRT;
            s.minValue = min; s.maxValue = max; s.value = (min + max) / 2f;
            return s;
        }

        Texto("Titulo", new Vector2(22, -16), new Vector2(380, 30), "Perfil de virus", 22, claro, TextAnchor.MiddleLeft);
        var nombre = Texto("Nombre", new Vector2(22, -46), new Vector2(390, 22), "Virus: " + NombreCovid, 15, secundario, TextAnchor.MiddleLeft);

        var ui = panelGO.AddComponent<PanelVirusUI>();
        ui.txtNombre = nombre;

        float y = -80f;
        Text val;
        Texto("lblEmision", new Vector2(22, y), new Vector2(250, 22), "Emisión de aerosoles (carga viral)", 15, claro, TextAnchor.MiddleLeft);
        val = Texto("valEmision", new Vector2(300, y), new Vector2(108, 22), "0.45", 15, claro, TextAnchor.MiddleRight);
        ui.txtEmision = val; ui.sldEmision = Deslizador("sldEmision", new Vector2(22, y - 24), 0.05f, 2f);

        y -= 56f;
        Texto("lblRadio", new Vector2(22, y), new Vector2(250, 22), "Radio de influencia (tamaño de partícula)", 15, claro, TextAnchor.MiddleLeft);
        val = Texto("valRadio", new Vector2(300, y), new Vector2(108, 22), "2.5 m", 15, claro, TextAnchor.MiddleRight);
        ui.txtRadio = val; ui.sldRadio = Deslizador("sldRadio", new Vector2(22, y - 24), 1f, 6f);

        y -= 56f;
        Texto("lblAtenuacion", new Vector2(22, y), new Vector2(250, 22), "Atenuación ambiental (vida útil)", 15, claro, TextAnchor.MiddleLeft);
        val = Texto("valAtenuacion", new Vector2(300, y), new Vector2(108, 22), "0.90", 15, claro, TextAnchor.MiddleRight);
        ui.txtAtenuacion = val; ui.sldAtenuacion = Deslizador("sldAtenuacion", new Vector2(22, y - 24), 0.3f, 1f);

        y -= 56f;
        Texto("lblUmbral", new Vector2(22, y), new Vector2(250, 22), "Dosis infecciosa característica (k)", 15, claro, TextAnchor.MiddleLeft);
        val = Texto("valUmbral", new Vector2(300, y), new Vector2(108, 22), "1.20", 15, claro, TextAnchor.MiddleRight);
        ui.txtUmbral = val; ui.sldUmbral = Deslizador("sldUmbral", new Vector2(22, y - 24), 0.3f, 2000f);

        // Botón para volver al perfil por defecto
        var btnGO = new GameObject("btnRestablecerVirus");
        btnGO.transform.SetParent(panelGO.transform, false);
        btnGO.AddComponent<Image>().color = boton;
        var btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0, 1); btnRT.anchorMax = new Vector2(0, 1); btnRT.pivot = new Vector2(0, 1);
        btnRT.anchoredPosition = new Vector2(22, y - 60f); btnRT.sizeDelta = new Vector2(250f, 34f);
        ui.btnRestablecer = btnGO.AddComponent<Button>();
        var btnTxt = Texto("Text", Vector2.zero, Vector2.zero, "Restablecer a COVID-19", 15, oscuro, TextAnchor.MiddleCenter);
        btnTxt.transform.SetParent(btnGO.transform, false);
        var btnTxtRT = btnTxt.GetComponent<RectTransform>();
        btnTxtRT.anchorMin = Vector2.zero; btnTxtRT.anchorMax = Vector2.one;
        btnTxtRT.offsetMin = Vector2.zero; btnTxtRT.offsetMax = Vector2.zero;
    }
}
