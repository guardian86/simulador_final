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
    [Header("Simulaciones múltiples")]
    public int cantidadSimulacionesLote = 5;
    public float duracionSimulacionSegundos = 45f;
    public int aforoMinimoPorSimulacion = 20;
    public int aforoMaximoPorSimulacion = 60;
    [Tooltip("Usar Application.persistentDataPath para guardar reportes")] public bool usarRutaPortable = true;
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
        }

        if (string.IsNullOrWhiteSpace(rutaSalidaPersonalizada) && usarRutaPortable)
            rutaSalidaPersonalizada = ObtenerCarpetaPortablePorDefecto();

        if (autoSpawn)
        {
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
                    clon.SetActive(true);
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
                
                var estadoSalud = clon.GetComponent<EstadoSaludAgente>();
                if (estadoSalud != null)
                {
                    bool infectadoInicial = UnityEngine.Random.value <= probabilidadIngresoInfectado;
                    estadoSalud.ConfigurarEstadoInicial(infectadoInicial, umbralContagioAerosolBase);
                }

                cantidadAgentesActivos++;
                if (autoSpawn)
                    Invoke(nameof(CrearAgente), Mathf.Max(0.1f, intervaloSpawn));
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message.ToString());
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
            var estadisticas = ConstruirReporteSimulacionActual(1, AforoMaximo, duracionActual);
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

        if (usarRutaPortable)
            return ObtenerCarpetaPortablePorDefecto();

        return Constantes.folderPath;
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

        var reporte = new estadisticacontagiocovid
        {
            numeroSimulacion = numeroSimulacion,
            aforoObjetivo = aforoObjetivo,
            duracionSimulacionSegundos = duracion,
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

        for (int indice = 1; indice <= cantidadSimulacionesLote; indice++)
        {
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
        tiempoSimulacionActual = 0f;
        simulacionEnCurso = true;
    }

    private string ConstruirResumenVisual(estadisticacontagiocovid reporte)
    {
        return $"Simulación {reporte.numeroSimulacion} | Aforo {reporte.aforoObjetivo} | Iniciales {reporte.infectadosIniciales} | Secundarios {reporte.contagiosSecundarios} | Final {reporte.promedioTotalContagio:0.0}%";
    }

    private string ConstruirResumenVisualLote(ResumenLoteSimulaciones resumen)
    {
        return $"Lote {resumen.cantidadSimulaciones} corridas | Prevalencia promedio {resumen.promedioPrevalenciaFinal:0.0}% | Secundario promedio {resumen.promedioPorcentajeContagioSecundario:0.0}%";
    }

    private string ObtenerCarpetaPortablePorDefecto()
    {
        return Path.Combine(Application.persistentDataPath, "ReporteAgentes");
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

        if (agente.GetComponent<AspectoAgente>() != null)
            return;

        var renders = agente.GetComponentsInChildren<MeshRenderer>(true)
            .Where(render => render.GetComponentInParent<ParticleSystem>() == null)
            .ToArray();

        bool esPrefabBasico = renders.Length <= 2;
        if (!esPrefabBasico)
            return;

        foreach (var render in renders)
            render.enabled = false;

        FabricaPersonaSimple.ConstruirPersona(agente);
    }

}
