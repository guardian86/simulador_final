using UnityEngine;
using UnityEngine.UI;

// Panel "Homologación": controles para replicar un escenario de referencia de grupo cerrado
// (por ejemplo el Aula de Lelieveld et al.): flujo normal de entrada y salida, caso índice presente de forma continua,
// aceleración del tiempo simulado y zona de destinos. Los valores se guardan en el Administrador
// y quedan registrados en el reporte JSON de cada ejecución.
public class PanelHomologacionUI : MonoBehaviour
{
    public Button btnGrupo;
    public Text txtGrupo;
    public Slider sldCasos, sldAceleracion, sldZona;
    public Text txtCasos, txtAceleracion, txtZona;

    private Administrador admin;
    private bool iniciado;

    void Start()
    {
        admin = FindObjectOfType<Administrador>();
        btnGrupo.onClick.AddListener(() =>
        {
            if (admin == null) return;
            admin.modoGrupoFijo = !admin.modoGrupoFijo;
            RefrescarTextos();
        });
        sldCasos.onValueChanged.AddListener(v => { if (admin != null) admin.casosInicialesFijos = Mathf.RoundToInt(v); RefrescarTextos(); });
        sldAceleracion.onValueChanged.AddListener(v => { if (admin != null) admin.EstablecerAceleracionTiempo(Mathf.RoundToInt(v)); RefrescarTextos(); });
        sldZona.onValueChanged.AddListener(v => { if (admin != null) admin.cantidadMetasZona = Mathf.RoundToInt(v); RefrescarTextos(); });
    }

    void Update()
    {
        if (admin == null)
        {
            admin = FindObjectOfType<Administrador>();
            return;
        }

        if (!iniciado)
        {
            sldCasos.SetValueWithoutNotify(admin.casosInicialesFijos);
            sldAceleracion.SetValueWithoutNotify(admin.aceleracionTiempo);
            sldZona.SetValueWithoutNotify(admin.cantidadMetasZona);
            RefrescarTextos();
            iniciado = true;
        }

        bool editable = !admin.LoteEnEjecucion;
        btnGrupo.interactable = editable;
        sldCasos.interactable = editable;
        sldAceleracion.interactable = editable;
        sldZona.interactable = editable;
    }

    void RefrescarTextos()
    {
        if (admin == null) return;
        txtGrupo.text = admin.modoGrupoFijo ? "Modo homologación: SÍ" : "Modo homologación: NO";
        txtCasos.text = admin.casosInicialesFijos <= 0 ? "Aleatorio" : admin.casosInicialesFijos.ToString();
        txtAceleracion.text = "x" + Mathf.RoundToInt(admin.aceleracionTiempo);
        txtZona.text = admin.cantidadMetasZona <= 0 ? "Todo" : admin.cantidadMetasZona.ToString();
    }

    public static void Crear(Transform canvas, Font fuente)
    {
        Color colorPanel = new Color(0.08f, 0.1f, 0.14f, 0.78f);
        Color claro = new Color(0.94f, 0.95f, 0.97f);
        Color oscuro = new Color(0.11f, 0.13f, 0.16f);
        Color sliderFondo = new Color(0.24f, 0.28f, 0.34f, 0.92f);
        Color sliderActivo = new Color(0.56f, 0.66f, 0.72f, 1f);
        Color boton = new Color(0.86f, 0.88f, 0.9f, 0.94f);

        var panelGO = new GameObject("PanelHomologacion");
        panelGO.transform.SetParent(canvas, false);
        panelGO.AddComponent<Image>().color = colorPanel;
        var panelRT = panelGO.GetComponent<RectTransform>();
        // Esquina superior izquierda, debajo del panel de control (a la derecha ya hay otros paneles y leyendas).
        panelRT.anchorMin = new Vector2(0f, 1f);
        panelRT.anchorMax = new Vector2(0f, 1f);
        panelRT.pivot = new Vector2(0f, 1f);
        panelRT.anchoredPosition = new Vector2(10f, -790f);
        panelRT.sizeDelta = new Vector2(430f, 250f);

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
            s.minValue = min; s.maxValue = max; s.wholeNumbers = true; s.value = min;
            return s;
        }

        Texto("Titulo", new Vector2(22, -10), new Vector2(380, 30), "Homologación con la referencia", 20, claro, TextAnchor.MiddleLeft);

        var ui = panelGO.AddComponent<PanelHomologacionUI>();

        var btnGO = new GameObject("btnGrupoCerrado");
        btnGO.transform.SetParent(panelGO.transform, false);
        btnGO.AddComponent<Image>().color = boton;
        var btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0, 1); btnRT.anchorMax = new Vector2(0, 1); btnRT.pivot = new Vector2(0, 1);
        btnRT.anchoredPosition = new Vector2(22, -44f); btnRT.sizeDelta = new Vector2(250f, 32f);
        ui.btnGrupo = btnGO.AddComponent<Button>();
        ui.txtGrupo = Texto("Text", Vector2.zero, Vector2.zero, "Modo homologación: NO", 15, oscuro, TextAnchor.MiddleCenter);
        ui.txtGrupo.transform.SetParent(btnGO.transform, false);
        var btnTxtRT = ui.txtGrupo.GetComponent<RectTransform>();
        btnTxtRT.anchorMin = Vector2.zero; btnTxtRT.anchorMax = Vector2.one;
        btnTxtRT.offsetMin = Vector2.zero; btnTxtRT.offsetMax = Vector2.zero;

        float y = -84f;
        Texto("lblCasos", new Vector2(22, y), new Vector2(290, 22), "Casos iniciales por corrida", 15, claro, TextAnchor.MiddleLeft);
        ui.txtCasos = Texto("valCasos", new Vector2(310, y), new Vector2(98, 22), "Aleatorio", 15, claro, TextAnchor.MiddleRight);
        ui.sldCasos = Deslizador("sldCasos", new Vector2(22, y - 24), 0f, 5f);

        y -= 50f;
        Texto("lblAceleracion", new Vector2(22, y), new Vector2(290, 22), "Aceleración del tiempo", 15, claro, TextAnchor.MiddleLeft);
        ui.txtAceleracion = Texto("valAceleracion", new Vector2(310, y), new Vector2(98, 22), "x1", 15, claro, TextAnchor.MiddleRight);
        ui.sldAceleracion = Deslizador("sldAceleracion", new Vector2(22, y - 24), 1f, 20f);

        y -= 50f;
        Texto("lblZona", new Vector2(22, y), new Vector2(290, 22), "Destinos de la zona", 15, claro, TextAnchor.MiddleLeft);
        ui.txtZona = Texto("valZona", new Vector2(310, y), new Vector2(98, 22), "Todo", 15, claro, TextAnchor.MiddleRight);
        ui.sldZona = Deslizador("sldZona", new Vector2(22, y - 24), 0f, 16f);
    }
}
