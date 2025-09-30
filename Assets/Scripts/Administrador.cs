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
    public int AforoMaximo;
    #endregion

    #region "Variables Privadas"
    int contadorAgentes = 0;
    private GameObject clon;
    #endregion


    // Update is called once per frame
    private void Start()
    {
        Invoke("CrearAgente", 2f);
    }

    void Update() { }

    void CrearAgente()
    {
        try
        {
            if (contadorAgentes < AforoMaximo)
            {

                clon = Instantiate(persona, puntoInicio.transform.position, puntoInicio.transform.rotation);
                clon.GetComponentInChildren<Camino>().enabled = true;
                clon.GetComponentInChildren<Particula>().enabled = true;
                clon.gameObject.SetActive(true);
                
                var probCovid = UnityEngine.Random.Range(0, 100);

                if (probCovid > 80) clon.GetComponentInChildren<ParticleSystem>().Play(true);
                else clon.GetComponentInChildren<ParticleSystem>().Stop(true);


                //Debug.Log("CrearAgente " + contadorAgentes);
                contadorAgentes++;
                Invoke("CrearAgente", 2f);
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

}
