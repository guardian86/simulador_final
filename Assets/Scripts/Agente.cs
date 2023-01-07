using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ParticleSystem;

public class Agente : MonoBehaviour
{

    #region "Variables de Entorno"
    private EmissionModule AgenteEmisor;

    int contadorCovid = 0;
    int contadorCovidF = 0;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ObtenerEstadoInicialAgentes", 12);
    }

    public void ObtenerEstadoInicialAgentes()
    {
        GameObject[] listaAgentes = GameObject.FindGameObjectsWithTag("tagPersonas");
        

        foreach (var agente in listaAgentes)
        {
            var estadoParticulaAgente = agente.GetComponentInChildren<ParticleSystem>().isEmitting;
            Globales.agenteCovid19.Add(new AgentesContagiados() 
            { 
                cantidadContagiados = contadorCovid, 
                tieneCovid = estadoParticulaAgente 
            });

            contadorCovid++;
            if (estadoParticulaAgente)
            {
                agente.GetComponentInChildren<ParticleSystem>().Stop(true);
            }
        }
        
        contadorCovid = 0;
        Invoke("ReactivaParticulaAgentes", Random.Range(0, 8));

        //Invoke("ObtenerEstadoIncialAgentes", 5);
        //Vector3 v = listaAgentes[salidaEscogida].transform.position;
        //this.GetComponent<NavMeshAgent>().SetDestination(v);
        //Invoke("CrearAgente", 2f);
    }

    //GameObject[] listaAgentes, List<AgentesContagiados> agentes
    public void ReactivaParticulaAgentes()
    {
        GameObject[] listaAgentes = GameObject.FindGameObjectsWithTag("tagPersonas");
        //System.Threading.Thread.Sleep(10000);
        foreach (var item in Globales.agenteCovid19)
        {
            if (item.tieneCovid)
            {
                listaAgentes[item.cantidadContagiados].GetComponentInChildren<ParticleSystem>().Play(true);
            }
        }
        Invoke("ObtenerEstadoInicialAgentes", Random.Range(0,8));
    }

    // Update is called once per frame
    void Update()
    {
        //if (contadorAgentes > 1) Invoke("ActualizarEstadoParticulaClon", 6f);
    }

    #region "Manejo Eventos y Agentes"
  
    private void ActivarDispararParticula()
    {
        if (!AgenteEmisor.enabled && Globales.agenteCovid19.Any())
            AgenteEmisor.enabled = true;
        //else if (agenteCovid19.Any())
        //    AgenteEmisor.enabled = true;
        else
            AgenteEmisor.enabled = false;
    }


    public void ActualizarEstadoParticulaXAgente()
    {

    }

    public void ActualizarEstadoParticulaClon()
    {

        try
        {
            //if (contadorAgentes > 1)
            //{
                
                //AgenteEmisor = clon.GetComponentInChildren<ParticleSystem>().emission;
                if (AgenteEmisor.enabled) //IsUnityNull() AgenteEmisor.ToString().Count() > 0
                {
                    Globales.agenteCovid19.Add(new AgentesContagiados() { cantidadContagiados = contadorCovidF, tieneCovid = true });
                    contadorCovidF++;

                    //ActivarDispararParticula();
                    AgenteEmisor.enabled = false;
                    Invoke("ActivarDispararParticula", Random.Range(0, 4));
                }
                else
                {
                    Globales.agenteCovid19.Add(new AgentesContagiados() { cantidadContagiados = 0, tieneCovid = false });
                }
            //}
        }
        catch (System.Exception ex)
        {

            Debug.LogError(ex.Message.ToString());
        }
    }

    #endregion

    public class AgentesContagiados
    {
        public bool tieneCovid { get; set; }
        public int cantidadContagiados { get; set; }
    }

}
