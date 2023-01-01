using System;
using System.IO;
using System.Linq;
using UnityEngine;
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
    ReporteAgentes reporteAgentes;
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
            if (contadorAgentes < AforoMaximo )
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


    private void ObtenerReporteAgentes()
    {
        listAgentes = GameObject.FindGameObjectsWithTag("tagPersonas");
        contAgentesCovid = listAgentes.Count();

        if (contAgentesCovid > 0)
        {

        }
        foreach (GameObject agente in listAgentes)
        {
            //agente.SetActive(true);
            emision = agente.gameObject.GetComponent<ParticleSystem>().emission;
            if (emision.enabled)
            {
                contAgentesCovid++;
            }
        }
        //var x = listAgentes[listAgentes.Length - 1].transform.gameObject.active;
        //ParticleSystem[] y = emision = ParticleSystem.FindObjectsOfType<ParticleSystem>();

        //emision = GetComponent<List<ParticleSystem>>().Count();

        if (listAgentes.Any())
        {
            using (StreamReader r = new StreamReader("Assets/JSON/ReporteAgentes.json"))
            {
                json = r.ReadToEnd();
                Debug.Log(json);
                Administrador x = CreateFromJSON(json);

                //Debug.Log(x[""][""]);
            }
        }
        
    }


    public void Load(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, this);
    }


    //generar el json a exportar 
    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
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


}
