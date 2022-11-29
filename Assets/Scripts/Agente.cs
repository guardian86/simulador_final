using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Agente : MonoBehaviour
{

    #region "Variables de Entorno"
    public GameObject persona;
    public GameObject puntoInicio;
    private EmissionModule AgenteEmisor;
    private GameObject clon;
    public int AforoMaximo;
    int contadorAgentes = 0;
    List<AgentesContagiados> agenteCovid19;
    int contadorCovid = 1;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Invoke("CrearAgente", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (contadorAgentes > 1) Invoke("ActualizarEstadoParticulaClon", 6f);
    }

    #region "Manejo Eventos y Agentes"
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
                var probCovid = Random.Range(0, 100);

                if (probCovid > 80) clon.GetComponentInChildren<ParticleSystem>().Play(true);
                else clon.GetComponentInChildren<ParticleSystem>().Stop(true);

                Debug.Log("CrearAgente " + contadorAgentes);
                contadorAgentes++;
                Invoke("CrearAgente", 2f);
            }

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message.ToString());
        }
    }

    private void ActivarDispararParticula()
    {
        if (!AgenteEmisor.enabled && agenteCovid19.Any())
            AgenteEmisor.enabled = true;
        //else if (agenteCovid19.Any())
        //    AgenteEmisor.enabled = true;
        else
            AgenteEmisor.enabled = false;
    }

    public void ActualizarEstadoParticulaClon()
    {

        try
        {
            agenteCovid19 = new List<AgentesContagiados>();
            if (contadorAgentes > 1)
            {
                
                AgenteEmisor = clon.GetComponentInChildren<ParticleSystem>().emission;
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
            }
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
