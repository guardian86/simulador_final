using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Administrador;
using static UnityEngine.ParticleSystem;




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
    int contAgentesCovid = 0;
    EmissionModule emision;
    private string json;
    //List<ReporteAgentes> reporteAgentes;
    List<estadisticacontagiocovid> estadisticaContagioCovid;
    GameObject[] listAgentes;
    #endregion


    // Update is called once per frame
    private void Start()
    {
        Invoke("CrearAgente", 2f);
    }

    void Update()
    {


    }

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

                //clon.GetComponentInChildren<ParticleSystem>().Play(true);
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


    internal void ObtenerReporteAgentes()
    {
        try
        {
            float contadorAgentReport = 0f;
            var estadisticaContagioCovi = new estadisticacontagiocovid();
            var repotesagente = new ReporteAgentes();
            listAgentes = GameObject.FindGameObjectsWithTag("tagPersonas");
            contAgentesCovid = listAgentes.Count();
            float promediotoal = 0;
            estadisticaContagioCovi.reporteAgentes = new List<ReporteAgentes>();
            foreach (var agenteRpt in listAgentes)
            {
                estadisticaContagioCovi.reporteAgentes.Add(new ReporteAgentes()
                {
                    agenteContagiadoCovid = agenteRpt.GetComponentInChildren<ParticleSystem>().isEmitting,
                    cantidadSimulaciones = contadorAgentReport += 1,
                    cantidadAgenteSimulacion = contadorAgentReport,
                    promedioContagiados = agenteRpt.GetComponentInChildren<ParticleSystem>().isEmitting ? (1f * contAgentesCovid) / 100f : 0f,
                });
                promediotoal += agenteRpt.GetComponentInChildren<ParticleSystem>().isEmitting ? (1f * contAgentesCovid) / 100f : 0f;
            }
            estadisticaContagioCovi.promedioTotalContagio = promediotoal;

            SaveRptJson(estadisticaContagioCovi);
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
            
            var json = JsonConvert.SerializeObject(rptAgent);
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
