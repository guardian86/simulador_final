using UnityEngine;
using UnityEngine.UI;

// Crea automáticamente un Canvas con controles básicos y conecta SimuladorUI
public class UIBootstrap : MonoBehaviour
{
    void Start()
    {
        if (FindObjectOfType<SimuladorUI>() != null) return;

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
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel
        var panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform);
        var panelImg = panelGO.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.4f);
        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0, 1);
        panelRT.anchorMax = new Vector2(0, 1);
        panelRT.pivot = new Vector2(0, 1);
        panelRT.anchoredPosition = new Vector2(10, -10);
        panelRT.sizeDelta = new Vector2(360, 240);

        // Helper de creación de botón
        Button CrearBoton(string nombre, Vector2 pos, string texto)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(panelGO.transform);
            var img = go.AddComponent<Image>();
            img.color = new Color(1,1,1,0.9f);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 28);
            rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1); rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = pos;
            var btn = go.AddComponent<Button>();
            var txtGO = new GameObject("Text");
            txtGO.transform.SetParent(go.transform);
            var txt = txtGO.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.text = texto; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.black;
            var txtrt = txtGO.GetComponent<RectTransform>(); txtrt.anchorMin = Vector2.zero; txtrt.anchorMax = Vector2.one; txtrt.offsetMin = Vector2.zero; txtrt.offsetMax = Vector2.zero;
            return btn;
        }

        Text CrearLabel(string nombre, Vector2 pos, string texto)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(panelGO.transform);
            var txt = go.AddComponent<Text>();
            txt.text = texto; txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); txt.color = Color.white; txt.alignment = TextAnchor.MiddleLeft;
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(330, 20);
            rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = pos;
            return txt;
        }

        Slider CrearSlider(string nombre, Vector2 pos, float min, float max)
        {
            var go = new GameObject(nombre);
            go.transform.SetParent(panelGO.transform);
            var bg = go.AddComponent<Image>();
            bg.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(180, 18);
            rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = pos;

            var fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(go.transform);
            var fill = fillGO.AddComponent<Image>(); fill.color = new Color(0.2f,0.6f,1f,0.9f);
            var fillRT = fillGO.GetComponent<RectTransform>(); fillRT.anchorMin = new Vector2(0,0); fillRT.anchorMax = new Vector2(1,1); fillRT.offsetMin = Vector2.zero; fillRT.offsetMax = Vector2.zero;

            var handleGO = new GameObject("Handle");
            handleGO.transform.SetParent(go.transform);
            var handle = handleGO.AddComponent<Image>(); handle.color = Color.white;
            var handleRT = handleGO.GetComponent<RectTransform>(); handleRT.sizeDelta = new Vector2(10, 18);

            var slider = go.AddComponent<Slider>();
            slider.fillRect = fillRT;
            slider.targetGraphic = handle;
            slider.handleRect = handleRT;
            slider.minValue = min; slider.maxValue = max;
            slider.value = (min+max)/2f;
            return slider;
        }

        // Crear UI elements
        var lblResumen = CrearLabel("lblResumen", new Vector2(10, -30), "Agentes: 0 | Infectados: 0 (0%)");

        var lblAforo = CrearLabel("lblAforo", new Vector2(10, -60), "Aforo: ");
        var sldAforo = CrearSlider("sldAforo", new Vector2(100, -60), 1, 300);

        var lblIntervalo = CrearLabel("lblIntervalo", new Vector2(10, -90), "Spawn/s: ");
        var sldIntervalo = CrearSlider("sldIntervalo", new Vector2(100, -90), 0.1f, 5f);

        var lblProb = CrearLabel("lblProbContagio", new Vector2(10, -120), "Prob. Contagio: ");
        var sldProb = CrearSlider("sldProbContagio", new Vector2(150, -120), 0f, 1f);

        var btnIniciar = CrearBoton("btnIniciar", new Vector2(10, -160), "Iniciar");
        var btnDetener = CrearBoton("btnDetener", new Vector2(120, -160), "Detener");
        var btnReset = CrearBoton("btnReset", new Vector2(230, -160), "Reset");
        var btnExportar = CrearBoton("btnExportar", new Vector2(10, -195), "Exportar");

        // SimuladorUI
        var ui = panelGO.AddComponent<SimuladorUI>();
        ui.btnIniciar = btnIniciar;
        ui.btnDetener = btnDetener;
        ui.btnReset = btnReset;
        ui.btnExportar = btnExportar;
        ui.sldAforo = sldAforo;
        ui.sldIntervalo = sldIntervalo;
        ui.sldProbContagio = sldProb;
        ui.lblAforo = lblAforo;
        ui.lblIntervalo = lblIntervalo;
        ui.lblProbContagio = lblProb;
        ui.lblResumen = lblResumen;
    }
}
