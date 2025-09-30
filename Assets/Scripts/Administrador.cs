using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    [Range(0f,1f)] public float probContagioDefault = 0.25f;
    #endregion

    #region "Variables Privadas"
    int contadorAgentes = 0;
    private GameObject clon;
    #endregion


    // Update is called once per frame
    private void Start()
    {
        if (autoSpawn)
            Invoke(nameof(CrearAgente), Mathf.Max(0.1f, intervaloSpawn));
    }

    void Update() { }

    void CrearAgente()
    {
        try
        {
            if (contadorAgentes < AforoMaximo)
            {
                if (persona == null)
                {
                    persona = CrearAgentePorDefecto();
                }

                var pos = puntoInicio != null ? puntoInicio.transform.position : Vector3.zero;
                var rot = puntoInicio != null ? puntoInicio.transform.rotation : Quaternion.identity;
                clon = Instantiate(persona, pos, rot);
                clon.GetComponentInChildren<Camino>().enabled = true;
                clon.GetComponentInChildren<Particula>().enabled = true;
                var p = clon.GetComponentInChildren<Particula>();
                if (p != null) p.probContagio = probContagioDefault;
                clon.gameObject.SetActive(true);
                
                var probCovid = UnityEngine.Random.Range(0, 100);

                if (probCovid > 80) clon.GetComponentInChildren<ParticleSystem>().Play(true);
                else clon.GetComponentInChildren<ParticleSystem>().Stop(true);


                contadorAgentes++;
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
        try
        {
            var listaAgentes = GameObject.FindGameObjectsWithTag("tagPersonas");
            var estadisticas = new estadisticacontagiocovid { reporteAgentes = new List<ReporteAgentes>() };

            int total = listaAgentes.Length;
            int infectados = 0;
            int idx = 1;
            foreach (var go in listaAgentes)
            {
                var ps = go.GetComponentInChildren<ParticleSystem>();
                bool tiene = ps != null && ps.isEmitting;
                if (tiene) infectados++;
                estadisticas.reporteAgentes.Add(new ReporteAgentes
                {
                    agenteContagiadoCovid = tiene,
                    cantidadAgenteSimulacion = idx,
                    cantidadSimulaciones = idx,
                    promedioContagiados = 0f // se calcula al final de forma global
                });
                idx++;
            }
            // promedio global de infectados en porcentaje (0..100)
            estadisticas.promedioTotalContagio = total > 0 ? (infectados * 100f) / total : 0f;

            SaveRptJson(estadisticas);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        

    }


    public void Load(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, this);
    }


    //generar el json a exportar 
    public void SaveRptJson(estadisticacontagiocovid rptAgent)
    {
        try
        {
            
            var json = JsonConvert.SerializeObject(rptAgent, Formatting.Indented);
            if (!Directory.Exists(Constantes.folderPath))
                Directory.CreateDirectory(Constantes.folderPath);
            
            File.WriteAllText(String.Concat(Constantes.path, $"-{DateTime.Now.ToString("ddMMyyyyhhmmss")}.json"), json);

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message.ToString());
        }
        

    }


    public static Administrador CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Administrador>(jsonString);
    }




    [Serializable]
    public class ReporteAgentes
    {
        public bool agenteContagiadoCovid { get; set; }
        public float cantidadAgenteSimulacion { get; set; }
        public float promedioContagiados { get; set; }
        public float cantidadSimulaciones { get; set; }
    }

    [Serializable]
    public class estadisticacontagiocovid
    {
        public List<ReporteAgentes> reporteAgentes { get; set; }
        public float promedioTotalContagio { get; set; }
    }

    // Reinicia la simulación: elimina agentes y resetea contadores/estado
    public void ResetSimulacion()
    {
        CancelInvoke(nameof(CrearAgente));
        var agentes = GameObject.FindGameObjectsWithTag("tagPersonas");
        foreach (var a in agentes)
        {
            Destroy(a);
        }
        contadorAgentes = 0;
        Globales.agenteCovid19.Clear();
        if (autoSpawn)
            Invoke(nameof(CrearAgente), Mathf.Max(0.1f, intervaloSpawn));
    }

    public void IniciarSpawn()
    {
        autoSpawn = true;
        CancelInvoke(nameof(CrearAgente));
        Invoke(nameof(CrearAgente), Mathf.Max(0.1f, intervaloSpawn));
    }

    public void DetenerSpawn()
    {
        autoSpawn = false;
        CancelInvoke(nameof(CrearAgente));
    }

    // Crea un GameObject de agente por defecto si no se asignó un prefab
    private GameObject CrearAgentePorDefecto()
    {
        var go = new GameObject("AgenteAuto");
        try { go.tag = "tagPersonas"; } catch { /* el tag debe existir */ }

        // Cuerpo visual (capsule)
        var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.transform.SetParent(go.transform);
        body.transform.localPosition = Vector3.zero;
        var bodyCollider = body.GetComponent<Collider>();
        if (bodyCollider != null) bodyCollider.isTrigger = false;

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
        var camino = go.AddComponent<Camino>();
        camino.veloMax = 4;

        var particulaGO = new GameObject("Emisor");
        particulaGO.transform.SetParent(go.transform);
        particulaGO.transform.localPosition = new Vector3(0, 1.2f, 0);
        var ps = particulaGO.AddComponent<ParticleSystem>();
        var main = ps.main; main.startLifetime = 0.75f; main.startSpeed = 0.5f; main.startSize = 0.1f; main.simulationSpace = ParticleSystemSimulationSpace.World;
        var emission = ps.emission; emission.rateOverTime = 25f; emission.enabled = false; // se activa al contagiar
        var shape = ps.shape; shape.enabled = true; shape.shapeType = ParticleSystemShapeType.Cone; shape.angle = 25f; shape.radius = 0.1f;

        var p = particulaGO.AddComponent<Particula>();
        p.probContagio = probContagioDefault;

        return go;
    }

}
