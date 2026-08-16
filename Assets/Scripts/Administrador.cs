using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;




[Serializable]
public class Administrador : MonoBehaviour
{

    /// <summary>
    /// Funciones que va a realizar el administrador
    /// Crear unicamente el Agente 
    /// Llevar el control de los agentes para generar el reporte json con la info recibida
    /// </summary>

    #region "Variables de Publicas"
    public GameObject persona;
    public GameObject puntoInicio;
    public int AforoMaximo = 30;
    [Tooltip("Segundos entre spawns")] public float intervaloSpawn = 2f;
    [Tooltip("Si está activo, el administrador seguirá creando agentes hasta el aforo máximo")] public bool autoSpawn = true;
    [Header("Modelo aerosol")]
    [Range(0f,1f)] public float probabilidadIngresoInfectado = 0.2f;
    public float umbralContagioAerosolBase = 1.2f;
    [Range(0f,12f)] public float nivelVentilacionACH = 6f;
    [Range(0f,100f)] public float eficaciaMascarillaPorcentaje = 0f;
    [Header("Simulaciones múltiples")]
    public int cantidadSimulacionesLote = 5;
    public float duracionSimulacionSegundos = 45f;
    public int aforoMinimoPorSimulacion = 20;
    public int aforoMaximoPorSimulacion = 60;
    [Tooltip("Usar la carpeta Resultados_Simulador_Quintero del Escritorio para guardar reportes")] public bool usarRutaPortable = true;
    [Tooltip("Ruta personalizada de salida para los reportes JSON")]
    public string rutaSalidaPersonalizada = string.Empty;
    [Tooltip("Nombre base del archivo exportado")]
    public string nombreBaseArchivo = "RptAgentes";
    [Header("Config (opcional)")] public SimuladorConfig config;
    #endregion

    #region "Variables Privadas"
    int cantidadAgentesActivos = 0;
    private GameObject clon;
    private readonly Queue<GameObject> pool = new Queue<GameObject>();
    private Coroutine rutinaLote;
    private bool simulacionEnCurso;
    private float tiempoSimulacionActual;
    private int aforoObjetivoContextoActual;
    private float intervaloSpawnContextoActual;
    private float umbralContagioAerosolBaseContextoActual;
    private float nivelVentilacionACHContextoActual;
    private float eficaciaMascarillaPorcentajeContextoActual;
    private int semillaAleatoriaContextoActual;
    private int semillaAleatoriaLoteActual;
    #endregion

    public string UltimaRutaExportada { get; private set; }
    public string EstadoLoteActual { get; private set; } = "Listo";
    public bool LoteEnEjecucion => rutinaLote != null;
    public string ResumenVisualActual { get; private set; } = "Sin resultados todavía.";
    public float TiempoSimulacionActual => tiempoSimulacionActual;


    // Update is called once per frame
    private void Start()
    {
        // Cargar parámetros desde config si está asignada
        if (config != null)
        {
            AforoMaximo = config.aforoMaximo;
            intervaloSpawn = config.intervaloSpawn;
            autoSpawn = config.autoSpawn;
            usarRutaPortable = config.usarRutaPortable;
            cantidadSimulacionesLote = config.cantidadSimulacionesLote;
            duracionSimulacionSegundos = config.duracionSimulacionSegundos;
            aforoMinimoPorSimulacion = config.aforoMinimoPorSimulacion;
            aforoMaximoPorSimulacion = config.aforoMaximoPorSimulacion;
            probabilidadIngresoInfectado = config.probabilidadIngresoInfectado;
            umbralContagioAerosolBase = config.umbralContagioAerosolBase;
            nivelVentilacionACH = config.nivelVentilacionACH;
            eficaciaMascarillaPorcentaje = config.eficaciaMascarillaPorcentaje;
        }

        if (string.IsNullOrWhiteSpace(rutaSalidaPersonalizada) && usarRutaPortable)
            rutaSalidaPersonalizada = ObtenerCarpetaPortablePorDefecto();

        if (autoSpawn)
        {
            AplicarSemillaSimulacion(GenerarSemillaAleatoria());
            IniciarRelojSimulacion();
            Invoke(nameof(CrearAgente), Mathf.Max(0.1f, intervaloSpawn));
        }
    }

    void Update()
    {
        if (simulacionEnCurso)
            tiempoSimulacionActual += Time.deltaTime;
    }

    void CrearAgente()
    {
        try
        {
            if (cantidadAgentesActivos < Mathf.Max(1, AforoMaximo))
            {
                if (persona == null)
                {
                    persona = CrearAgentePorDefecto();
                }

                var pos = puntoInicio != null ? puntoInicio.transform.position : Vector3.zero;
                var rot = puntoInicio != null ? puntoInicio.transform.rotation : Quaternion.identity;

                if (pool.Count > 0)
                {
                    clon = pool.Dequeue();
                    clon.transform.SetPositionAndRotation(pos, rot);

                    // FIX: el NavMeshAgent vive en el hijo ("Capsule"), no en este
                    // objeto raíz, así que durante su vida anterior el hijo pudo
                    // haberse alejado mucho (offset local grande) mientras el padre
                    // se quedaba quieto. Si no se reinicia ese offset, al reciclar
                    // el agente del pool reaparece "teletransportado" lejos del
                    // punto de spawn real en vez de justo ahí.
                    foreach (Transform hijo in clon.transform)
                    {
                        hijo.localPosition = Vector3.zero;
                        hijo.localRotation = Quaternion.identity;
                    }

                    clon.SetActive(true);

                    foreach (Transform hijo in clon.transform)
                    {
                        var navAgenteHijo = hijo.GetComponent<UnityEngine.AI.NavMeshAgent>();
                        if (navAgenteHijo != null && navAgenteHijo.isOnNavMesh)
                            navAgenteHijo.Warp(pos);
                    }

                    AsegurarVisualPersona(clon);
                    var camino = clon.GetComponentInChildren<Camino>();
                    if (camino != null) camino.ReiniciarRuta();
                    var part = clon.GetComponentInChildren<Particula>();
                    if (part != null)
                    {
                        part.enabled = true;
                    }
                }
                else
                {
                    clon = Instantiate(persona, pos, rot);
                    AsegurarVisualPersona(clon);
                    var camino = clon.GetComponentInChildren<Camino>();
                    if (camino != null) camino.enabled = true;
                    var part = clon.GetComponentInChildren<Particula>();
                    if (part != null)
                    {
                        part.enabled = true;
                    }
                    clon.gameObject.SetActive(true);
                }

                var estadoSalud = clon.GetComponentInChildren<EstadoSaludAgente>();
                if (estadoSalud != null)
                {
                    bool infectadoInicial = UnityEngine.Random.value <= probabilidadIngresoInfectado;
                    estadoSalud.ConfigurarEstadoInicial(infectadoInicial, umbralContagioAerosolBase);
                }

                cantidadAgentesActivos++;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message.ToString());
        }
        finally
        {
            // Reprograma la próxima verificación de spawn siempre que el auto-spawn esté activo,
            // incluso si el aforo ya estaba lleno en este tick. Antes, el reintento solo se
            // programaba dentro del bloque "if" de creación, así que al llegar al aforo máximo
            // la cadena de Invoke se detenía para siempre y, aunque luego un agente saliera y
            // liberara cupo, nunca se volvía a intentar crear un reemplazo.
            if (autoSpawn)
                Invoke(nameof(CrearAgente), Mathf.Max(0.1f, intervaloSpawn));
        }
    }


    public void ObtenerReporteAgentes()
    {
        ExportarReporteActual();
    }

    public string ExportarReporteActual()
    {
        try
        {
            float duracionActual = tiempoSimulacionActual > 0.1f ? tiempoSimulacionActual : duracionSimulacionSegundos;
            var estadisticas = ConstruirReporteSimulacionActual(1, ObtenerAforoObjetivoParaReporte(), duracionActual);
            UltimaRutaExportada = GuardarJson(estadisticas, nombreBaseArchivo);
            ResumenVisualActual = ConstruirResumenVisual(estadisticas);
            return UltimaRutaExportada;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return string.Empty;
        }
    }


    public void Load(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, this);
    }


    //generar el json a exportar 
    public string SaveRptJson(estadisticacontagiocovid rptAgent)
    {
        return GuardarJson(rptAgent, nombreBaseArchivo);
    }


    public static Administrador CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Administrador>(jsonString);
    }




    [Serializable]
    public class ReporteAgentes
    {
        public bool agenteContagiadoCovid { get; set; }
        public bool casoInicial { get; set; }
        public bool contagioSecundario { get; set; }
        public float cantidadAgenteSimulacion { get; set; }
        public float promedioContagiados { get; set; }
        public float cantidadSimulaciones { get; set; }
        public float dosisAcumulada { get; set; }
        public float tiempoExposicionSegundos { get; set; }
    }

    [Serializable]
    public class estadisticacontagiocovid
    {
        public int numeroSimulacion { get; set; }
        public int aforoObjetivo { get; set; }
        public float duracionSimulacionSegundos { get; set; }
        public float intervaloSpawn { get; set; }
        public float umbralContagioAerosolBase { get; set; }
        [JsonProperty("NivelVentilacionACH")] public float nivelVentilacionACH { get; set; }
        [JsonProperty("EficaciaMascarillaPorcentaje")] public float eficaciaMascarillaPorcentaje { get; set; }
        public int semillaAleatoria { get; set; }
        public int infectadosIniciales { get; set; }
        public int infectadosFinales { get; set; }
        public int contagiosSecundarios { get; set; }
        public float porcentajeContagioSecundario { get; set; }
        public List<ReporteAgentes> reporteAgentes { get; set; }
        public float promedioTotalContagio { get; set; }
    }

    [Serializable]
    public class ResumenLoteSimulaciones
    {
        public int cantidadSimulaciones { get; set; }
        public float intervaloSpawn { get; set; }
        public float umbralContagioAerosolBase { get; set; }
        [JsonProperty("NivelVentilacionACH")] public float nivelVentilacionACH { get; set; }
        [JsonProperty("EficaciaMascarillaPorcentaje")] public float eficaciaMascarillaPorcentaje { get; set; }
        public int semillaAleatoria { get; set; }
        public string rutaCarpetaSalida { get; set; }
        public List<estadisticacontagiocovid> simulaciones { get; set; }
        public float promedioPorcentajeContagioSecundario { get; set; }
        public float promedioPrevalenciaFinal { get; set; }
    }

    // Reinicia la simulación: elimina agentes y resetea contadores/estado
    public void ResetSimulacion()
    {
        ResetSimulacionInterna(autoSpawn);
    }

    private void ResetSimulacionInterna(bool reiniciarLuego)
    {
        CancelInvoke(nameof(CrearAgente));
        var agentes = GameObject.FindGameObjectsWithTag("tagPersonas");
        foreach (var a in agentes)
        {
            ReleaseAgente(a);
        }
        cantidadAgentesActivos = 0;
        Globales.agenteCovid19.Clear();
        Globales.generaRpt = true;
        tiempoSimulacionActual = 0f;
        simulacionEnCurso = reiniciarLuego;
        if (reiniciarLuego)
        {
            EstadoLoteActual = "Simulación reiniciada";
            Invoke(nameof(CrearAgente), Mathf.Max(0.1f, intervaloSpawn));
        }
    }

    public void IniciarSpawn()
    {
        autoSpawn = true;
        CancelInvoke(nameof(CrearAgente));
        AplicarSemillaSimulacion(GenerarSemillaAleatoria());
        IniciarRelojSimulacion();
        Invoke(nameof(CrearAgente), Mathf.Max(0.1f, intervaloSpawn));
        EstadoLoteActual = "Simulación en ejecución";
    }

    public void DetenerSpawn()
    {
        autoSpawn = false;
        CancelInvoke(nameof(CrearAgente));
        simulacionEnCurso = false;
        if (rutinaLote != null)
        {
            StopCoroutine(rutinaLote);
            rutinaLote = null;
            EstadoLoteActual = "Lote cancelado";
        }
    }

    public void EstablecerAforoMaximo(int nuevoAforo)
    {
        AforoMaximo = Mathf.Max(1, nuevoAforo);
    }

    public void EstablecerIntervaloSpawn(float nuevoIntervalo)
    {
        intervaloSpawn = Mathf.Clamp(nuevoIntervalo, 0.1f, 30f);
        if (autoSpawn)
        {
            CancelInvoke(nameof(CrearAgente));
            Invoke(nameof(CrearAgente), intervaloSpawn);
        }
    }

    public void EstablecerCantidadSimulaciones(int nuevaCantidad)
    {
        cantidadSimulacionesLote = Mathf.Max(1, nuevaCantidad);
    }

    public void EstablecerDuracionSimulacion(float nuevaDuracion)
    {
        duracionSimulacionSegundos = Mathf.Clamp(nuevaDuracion, 5f, 600f);
    }

    public void EstablecerNivelVentilacion(float nuevoNivelACH)
    {
        nivelVentilacionACH = Mathf.Clamp(nuevoNivelACH, 0f, 12f);
    }

    public void EstablecerEficaciaMascarilla(float nuevaEficaciaPorcentaje)
    {
        eficaciaMascarillaPorcentaje = Mathf.Clamp(nuevaEficaciaPorcentaje, 0f, 100f);
    }

    public float ObtenerFactorMascarillaAerosoles()
    {
        return 1f - Mathf.Clamp01(eficaciaMascarillaPorcentaje / 100f);
    }

    public float ObtenerFactorVentilacionAerosoles()
    {
        // La ventilacion se modela como una dilucion exponencial de aerosoles acumulados.
        return Mathf.Exp(-0.22f * Mathf.Clamp(nivelVentilacionACH, 0f, 12f));
    }

    public float ObtenerMultiplicadorMitigacionAerosoles()
    {
        return Mathf.Clamp01(ObtenerFactorMascarillaAerosoles() * ObtenerFactorVentilacionAerosoles());
    }

    public void EstablecerRangoAforoSimulacion(int minimo, int maximo)
    {
        aforoMinimoPorSimulacion = Mathf.Max(1, minimo);
        aforoMaximoPorSimulacion = Mathf.Max(aforoMinimoPorSimulacion, maximo);
    }

    public void EstablecerRutaSalida(string nuevaRuta)
    {
        rutaSalidaPersonalizada = NormalizarRuta(nuevaRuta);
        if (string.IsNullOrWhiteSpace(rutaSalidaPersonalizada) && usarRutaPortable)
            rutaSalidaPersonalizada = ObtenerCarpetaPortablePorDefecto();
    }

    public string ObtenerCarpetaSalida()
    {
        if (!string.IsNullOrWhiteSpace(rutaSalidaPersonalizada))
            return rutaSalidaPersonalizada;

        return ObtenerCarpetaPortablePorDefecto();
    }

    public int ObtenerCantidadAgentesActivos()
    {
        return GameObject.FindGameObjectsWithTag("tagPersonas").Count(agente => agente.activeInHierarchy);
    }

    public int ObtenerCantidadAgentesInfectados()
    {
        return GameObject.FindGameObjectsWithTag("tagPersonas")
            .Count(agente => agente.activeInHierarchy && agente.GetComponent<EstadoSaludAgente>() != null && agente.GetComponent<EstadoSaludAgente>().estaInfectado);
    }

    public int ObtenerCantidadCasosIniciales()
    {
        return GameObject.FindGameObjectsWithTag("tagPersonas")
            .Count(agente => agente.activeInHierarchy && agente.GetComponent<EstadoSaludAgente>() != null && agente.GetComponent<EstadoSaludAgente>().fueCasoInicial);
    }

    public int ObtenerCantidadContagiosSecundariosActuales()
    {
        return GameObject.FindGameObjectsWithTag("tagPersonas")
            .Count(agente => agente.activeInHierarchy && agente.GetComponent<EstadoSaludAgente>() != null && agente.GetComponent<EstadoSaludAgente>().fueContagioSecundario);
    }

    public void EjecutarLoteSimulaciones()
    {
        if (rutinaLote != null)
            return;

        rutinaLote = StartCoroutine(RutinaLoteSimulaciones());
    }

    public void AbrirCarpetaSalida()
    {
        string carpeta = ObtenerCarpetaSalida();
        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        string rutaUrl = carpeta.Replace("\\", "/");
        Application.OpenURL($"file:///{rutaUrl}");
    }

    // Crea un GameObject de agente por defecto si no se asignó un prefab
    private GameObject CrearAgentePorDefecto()
    {
        var go = new GameObject("AgenteAuto");
        try { go.tag = "tagPersonas"; } catch { /* el tag debe existir */ }

        // Aura de proximidad (trigger)
        var aura = new GameObject("AuraProximidad");
        aura.transform.SetParent(go.transform);
        aura.transform.localPosition = Vector3.zero;
        var col = aura.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 1.2f;

        // Física mínima
        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        // Navegación
        var nma = go.AddComponent<UnityEngine.AI.NavMeshAgent>();
        nma.angularSpeed = 120f;
        nma.acceleration = 8f;
        nma.stoppingDistance = 0.1f;

        // Scripts de lógica
        go.AddComponent<Agente>();
        go.AddComponent<EstadoSaludAgente>();
        var camino = go.AddComponent<Camino>();
        camino.veloMax = 4;

        var particulaGO = new GameObject("Emisor");
        particulaGO.transform.SetParent(go.transform);
        particulaGO.transform.localPosition = new Vector3(0, 1.2f, 0);
        var ps = particulaGO.AddComponent<ParticleSystem>();
        var main = ps.main; main.startLifetime = 0.75f; main.startSpeed = 0.5f; main.startSize = 0.1f; main.simulationSpace = ParticleSystemSimulationSpace.World;
        var emission = ps.emission; emission.rateOverTime = 25f; emission.enabled = false; // se activa al contagiar
        var shape = ps.shape; shape.enabled = true; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 25f; shape.radius = 0.1f;

        particulaGO.AddComponent<Particula>();
        FabricaPersonaSimple.ConstruirPersona(go);

        return go;
    }

    // Libera el agente (pool) en vez de destruir
    public void ReleaseAgente(GameObject agenteRoot)
    {
        if (agenteRoot == null) return;
        if (!agenteRoot.activeInHierarchy) return;

        // Apaga partículas y resetea
        var ps = agenteRoot.GetComponentInChildren<ParticleSystem>();
        if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var nav = agenteRoot.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null) nav.ResetPath();

        agenteRoot.SetActive(false);
        cantidadAgentesActivos = Mathf.Max(0, cantidadAgentesActivos - 1);
        pool.Enqueue(agenteRoot);
    }

    private estadisticacontagiocovid ConstruirReporteSimulacionActual(int numeroSimulacion, int aforoObjetivo, float duracion)
    {
        var estados = GameObject.FindGameObjectsWithTag("tagPersonas")
            .Where(agente => agente.activeInHierarchy)
            .Select(agente => agente.GetComponent<EstadoSaludAgente>())
            .Where(estado => estado != null)
            .ToList();

        int infectadosIniciales = estados.Count(estado => estado.fueCasoInicial);
        int infectadosFinales = estados.Count(estado => estado.estaInfectado);
        int contagiosSecundarios = estados.Count(estado => estado.fueContagioSecundario);
        int susceptiblesIniciales = Mathf.Max(0, estados.Count - infectadosIniciales);
        float intervaloSpawnReporte = ObtenerIntervaloSpawnParaReporte();
        float umbralContagioReporte = ObtenerUmbralContagioParaReporte();
        float nivelVentilacionReporte = ObtenerNivelVentilacionParaReporte();
        float eficaciaMascarillaReporte = ObtenerEficaciaMascarillaParaReporte();
        int semillaReporte = ObtenerSemillaParaReporte();

        var reporte = new estadisticacontagiocovid
        {
            numeroSimulacion = numeroSimulacion,
            aforoObjetivo = aforoObjetivo,
            duracionSimulacionSegundos = duracion,
            intervaloSpawn = intervaloSpawnReporte,
            umbralContagioAerosolBase = umbralContagioReporte,
            nivelVentilacionACH = nivelVentilacionReporte,
            eficaciaMascarillaPorcentaje = eficaciaMascarillaReporte,
            semillaAleatoria = semillaReporte,
            infectadosIniciales = infectadosIniciales,
            infectadosFinales = infectadosFinales,
            contagiosSecundarios = contagiosSecundarios,
            porcentajeContagioSecundario = susceptiblesIniciales > 0 ? (contagiosSecundarios * 100f) / susceptiblesIniciales : 0f,
            promedioTotalContagio = estados.Count > 0 ? (infectadosFinales * 100f) / estados.Count : 0f,
            reporteAgentes = new List<ReporteAgentes>()
        };

        int indice = 1;
        foreach (var estado in estados)
        {
            reporte.reporteAgentes.Add(new ReporteAgentes
            {
                agenteContagiadoCovid = estado.estaInfectado,
                casoInicial = estado.fueCasoInicial,
                contagioSecundario = estado.fueContagioSecundario,
                cantidadAgenteSimulacion = indice,
                cantidadSimulaciones = numeroSimulacion,
                promedioContagiados = reporte.promedioTotalContagio,
                dosisAcumulada = estado.dosisAcumulada,
                tiempoExposicionSegundos = estado.tiempoExposicionAcumulado
            });
            indice++;
        }

        return reporte;
    }

    private IEnumerator RutinaLoteSimulaciones()
    {
        EstadoLoteActual = "Preparando lote";
        bool autoSpawnOriginal = autoSpawn;
        var resultados = new List<estadisticacontagiocovid>();
        semillaAleatoriaLoteActual = GenerarSemillaAleatoria();

        for (int indice = 1; indice <= cantidadSimulacionesLote; indice++)
        {
            AplicarSemillaSimulacion(DerivarSemillaLote(indice));
            int aforoSimulacion = UnityEngine.Random.Range(aforoMinimoPorSimulacion, aforoMaximoPorSimulacion + 1);
            EstablecerAforoMaximo(aforoSimulacion);
            ResetSimulacionInterna(false);
            autoSpawn = true;
            IniciarRelojSimulacion();
            EstadoLoteActual = $"Simulación {indice}/{cantidadSimulacionesLote} en ejecución";
            Invoke(nameof(CrearAgente), Mathf.Max(0.1f, intervaloSpawn));
            yield return new WaitForSeconds(duracionSimulacionSegundos);
            CancelInvoke(nameof(CrearAgente));
            autoSpawn = false;
            simulacionEnCurso = false;
            yield return new WaitForSeconds(0.25f);

            var reporte = ConstruirReporteSimulacionActual(indice, aforoSimulacion, tiempoSimulacionActual);
            resultados.Add(reporte);
            GuardarJson(reporte, $"{nombreBaseArchivo}_Simulacion_{indice}");
            ResumenVisualActual = ConstruirResumenVisual(reporte);
            ResetSimulacionInterna(false);
            yield return null;
        }

        autoSpawn = autoSpawnOriginal;

        var resumen = new ResumenLoteSimulaciones
        {
            cantidadSimulaciones = resultados.Count,
            intervaloSpawn = intervaloSpawn,
            umbralContagioAerosolBase = umbralContagioAerosolBase,
            nivelVentilacionACH = nivelVentilacionACH,
            eficaciaMascarillaPorcentaje = eficaciaMascarillaPorcentaje,
            semillaAleatoria = semillaAleatoriaLoteActual,
            rutaCarpetaSalida = ObtenerCarpetaSalida(),
            simulaciones = resultados,
            promedioPorcentajeContagioSecundario = resultados.Count > 0 ? resultados.Average(item => item.porcentajeContagioSecundario) : 0f,
            promedioPrevalenciaFinal = resultados.Count > 0 ? resultados.Average(item => item.promedioTotalContagio) : 0f
        };

        UltimaRutaExportada = GuardarJson(resumen, $"{nombreBaseArchivo}_Lote");
        ResumenVisualActual = ConstruirResumenVisualLote(resumen);
        EstadoLoteActual = $"Lote finalizado. Archivo: {UltimaRutaExportada}";
        rutinaLote = null;
    }

    private void IniciarRelojSimulacion()
    {
        CapturarContextoSimulacionActual();
        tiempoSimulacionActual = 0f;
        simulacionEnCurso = true;
    }

    private string ConstruirResumenVisual(estadisticacontagiocovid reporte)
    {
        return $"Simulación {reporte.numeroSimulacion} | Semilla {reporte.semillaAleatoria} | ACH {reporte.nivelVentilacionACH:0.#} | Mascarilla {reporte.eficaciaMascarillaPorcentaje:0}% | Final {reporte.promedioTotalContagio:0.0}%";
    }

    private string ConstruirResumenVisualLote(ResumenLoteSimulaciones resumen)
    {
        return $"Lote {resumen.cantidadSimulaciones} corridas | Semilla base {resumen.semillaAleatoria} | ACH {resumen.nivelVentilacionACH:0.#} | Mascarilla {resumen.eficaciaMascarillaPorcentaje:0}% | Prevalencia promedio {resumen.promedioPrevalenciaFinal:0.0}%";
    }

    private void CapturarContextoSimulacionActual()
    {
        aforoObjetivoContextoActual = AforoMaximo;
        intervaloSpawnContextoActual = intervaloSpawn;
        umbralContagioAerosolBaseContextoActual = umbralContagioAerosolBase;
        nivelVentilacionACHContextoActual = nivelVentilacionACH;
        eficaciaMascarillaPorcentajeContextoActual = eficaciaMascarillaPorcentaje;
    }

    private void AplicarSemillaSimulacion(int semilla)
    {
        semillaAleatoriaContextoActual = semilla;
        UnityEngine.Random.InitState(semillaAleatoriaContextoActual);
    }

    private int GenerarSemillaAleatoria()
    {
        return unchecked((int)(DateTime.UtcNow.Ticks & 0x7FFFFFFF));
    }

    private int DerivarSemillaLote(int indiceSimulacion)
    {
        return unchecked(semillaAleatoriaLoteActual + (indiceSimulacion * 7919));
    }

    private int ObtenerAforoObjetivoParaReporte()
    {
        return simulacionEnCurso || tiempoSimulacionActual > 0.1f
            ? aforoObjetivoContextoActual
            : AforoMaximo;
    }

    private float ObtenerIntervaloSpawnParaReporte()
    {
        return simulacionEnCurso || tiempoSimulacionActual > 0.1f
            ? intervaloSpawnContextoActual
            : intervaloSpawn;
    }

    private float ObtenerUmbralContagioParaReporte()
    {
        return simulacionEnCurso || tiempoSimulacionActual > 0.1f
            ? umbralContagioAerosolBaseContextoActual
            : umbralContagioAerosolBase;
    }

    private float ObtenerNivelVentilacionParaReporte()
    {
        return simulacionEnCurso || tiempoSimulacionActual > 0.1f
            ? nivelVentilacionACHContextoActual
            : nivelVentilacionACH;
    }

    private float ObtenerEficaciaMascarillaParaReporte()
    {
        return simulacionEnCurso || tiempoSimulacionActual > 0.1f
            ? eficaciaMascarillaPorcentajeContextoActual
            : eficaciaMascarillaPorcentaje;
    }

    private int ObtenerSemillaParaReporte()
    {
        return semillaAleatoriaContextoActual != 0 ? semillaAleatoriaContextoActual : GenerarSemillaAleatoria();
    }

    private string ObtenerCarpetaPortablePorDefecto()
    {
        string escritorioUsuario = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string carpetaResultados = Path.Combine(escritorioUsuario, "Resultados_Simulador_Quintero");
        Directory.CreateDirectory(carpetaResultados);
        return carpetaResultados;
    }

    private string GuardarJson(object contenido, string nombreBase)
    {
        try
        {
            var json = JsonConvert.SerializeObject(contenido, Formatting.Indented);
            string baseFolder = ObtenerCarpetaSalida();
            string baseFile = Path.Combine(baseFolder, string.IsNullOrWhiteSpace(nombreBase) ? "RptAgentes" : nombreBase.Trim());
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            var rutaCompleta = string.Concat(baseFile, $"-{DateTime.Now:ddMMyyyyHHmmss}.json");
            File.WriteAllText(rutaCompleta, json);
            return rutaCompleta;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message.ToString());
        }
    }

    private string NormalizarRuta(string rutaOriginal)
    {
        if (string.IsNullOrWhiteSpace(rutaOriginal))
            return string.Empty;

        return rutaOriginal.Trim().Trim('"');
    }

    private void AsegurarVisualPersona(GameObject agente)
    {
        if (agente == null)
            return;

        var visualPersona = agente.transform.Find("VisualPersona");
        if (visualPersona != null)
        {
            visualPersona.gameObject.SetActive(true);
            foreach (var render in visualPersona.GetComponentsInChildren<Renderer>(true))
                render.enabled = true;
            return;
        }

        var renders = agente.GetComponentsInChildren<MeshRenderer>(true)
            .Where(render => render.GetComponentInParent<ParticleSystem>() == null)
            .ToArray();

        if (agente.GetComponent<AspectoAgente>() != null)
        {
            foreach (var render in renders)
                render.enabled = true;
            return;
        }

        bool esPrefabBasico = renders.Length <= 2;
        if (!esPrefabBasico)
        {
            foreach (var render in renders)
                render.enabled = true;
            return;
        }

        foreach (var render in renders)
            render.enabled = false;

        var aspecto = FabricaPersonaSimple.ConstruirPersona(agente);
        if (aspecto != null)
        {
            foreach (var render in agente.GetComponentsInChildren<Renderer>(true)
                .Where(render => render.GetComponentInParent<ParticleSystem>() == null))
            {
                render.enabled = true;
            }
        }
    }

}
