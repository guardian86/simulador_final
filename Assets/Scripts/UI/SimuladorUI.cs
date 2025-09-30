using UnityEngine;
using UnityEngine.UI;

public class SimuladorUI : MonoBehaviour
{
    public Administrador admin;

    [Header("Controles")]
    public Button btnIniciar;
    public Button btnDetener;
    public Button btnReset;
    public Button btnExportar;

    [Header("Parametros")]
    public Slider sldAforo;
    public Slider sldIntervalo;
    public Slider sldProbContagio;

    public Text lblAforo;
    public Text lblIntervalo;
    public Text lblProbContagio;
    public Text lblResumen;

    private void Start()
    {
        if (admin == null) admin = FindObjectOfType<Administrador>();

        if (btnIniciar) btnIniciar.onClick.AddListener(() => admin?.IniciarSpawn());
        if (btnDetener) btnDetener.onClick.AddListener(() => admin?.DetenerSpawn());
        if (btnReset) btnReset.onClick.AddListener(() => admin?.ResetSimulacion());
        if (btnExportar) btnExportar.onClick.AddListener(() => { admin?.ObtenerReporteAgentes(); ActualizarResumen(); });

        if (sldAforo)
        {
            sldAforo.minValue = 1; sldAforo.maxValue = 300;
            sldAforo.value = admin != null ? admin.AforoMaximo : 30;
            sldAforo.onValueChanged.AddListener(v => { if (admin) admin.AforoMaximo = Mathf.RoundToInt(v); ActualizarLabels(); });
        }
        if (sldIntervalo)
        {
            sldIntervalo.minValue = 0.1f; sldIntervalo.maxValue = 5f;
            sldIntervalo.value = admin != null ? admin.intervaloSpawn : 2f;
            sldIntervalo.onValueChanged.AddListener(v => { if (admin) admin.intervaloSpawn = v; ActualizarLabels(); });
        }
        if (sldProbContagio)
        {
            sldProbContagio.minValue = 0f; sldProbContagio.maxValue = 1f;
            sldProbContagio.value = admin != null ? admin.probContagioDefault : 0.25f;
            sldProbContagio.onValueChanged.AddListener(v => { if (admin) admin.probContagioDefault = v; ActualizarLabels(); });
        }

        ActualizarLabels();
        InvokeRepeating(nameof(ActualizarResumen), 1f, 1.5f);
    }

    private void ActualizarLabels()
    {
        if (!admin) return;
        if (lblAforo) lblAforo.text = $"Aforo: {admin.AforoMaximo}";
        if (lblIntervalo) lblIntervalo.text = $"Spawn/s: {admin.intervaloSpawn:0.0}s";
        if (lblProbContagio) lblProbContagio.text = $"Prob. Contagio: {admin.probContagioDefault:P0}";
    }

    private void ActualizarResumen()
    {
        var agentes = GameObject.FindGameObjectsWithTag("tagPersonas");
        int total = agentes.Length;
        int infectados = 0;
        foreach (var g in agentes)
        {
            var ps = g.GetComponentInChildren<ParticleSystem>();
            if (ps != null && ps.isEmitting) infectados++;
        }
        if (lblResumen) lblResumen.text = $"Agentes: {total} | Infectados: {infectados} ({(total>0?(infectados*100f/total):0f):0}% )";
    }
}
