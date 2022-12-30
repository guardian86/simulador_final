using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ParticleSystem;

public class Agente : MonoBehaviour
{

    #region "Variables de Entorno"
    private EmissionModule AgenteEmisor;
    
    
    
    List<AgentesContagiados> agenteCovid19;
    int contadorCovid = 1;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] listaAgentes = GameObject.FindGameObjectsWithTag("tagPersonas");
        int salidaEscogida = Random.Range(0, listaAgentes.Length);

        //Vector3 v = listaAgentes[salidaEscogida].transform.position;
        //this.GetComponent<NavMeshAgent>().SetDestination(v);
        //Invoke("CrearAgente", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        //if (contadorAgentes > 1) Invoke("ActualizarEstadoParticulaClon", 6f);
    }

    #region "Manejo Eventos y Agentes"
  
    private void ActivarDispararParticula()
    {
        if (!AgenteEmisor.enabled && agenteCovid19.Any())
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
            agenteCovid19 = new List<AgentesContagiados>();
            //if (contadorAgentes > 1)
            //{
                
                //AgenteEmisor = clon.GetComponentInChildren<ParticleSystem>().emission;
                if (AgenteEmisor.enabled) //IsUnityNull() AgenteEmisor.ToString().Count() > 0
                {
                    agenteCovid19.Add(new AgentesContagiados() { cantidadContagiados = contadorCovid, tieneCovid = true });
                    contadorCovid++;

                    //ActivarDispararParticula();
                    AgenteEmisor.enabled = false;
                    Invoke("ActivarDispararParticula", Random.Range(0, 4));
                }
                else
                {
                    agenteCovid19.Add(new AgentesContagiados() { cantidadContagiados = 0, tieneCovid = false });
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
