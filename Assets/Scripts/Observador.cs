using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observador : MonoBehaviour
{
    private bool aTieneCovid = false;
    public GameObject persona;
    public GameObject puntoInicio;
    public int AforoMaximo;
    int contadorAgentes = 0;


    // Update is called once per frame
    private void Start()
    {
        Invoke("CrearAgente", 2.0f);
    }
    void Update()
    {
        /*if (Input.GetKeyUp(KeyCode.Space))
        {
            CrearAgente();
        }*/
        

    }

    void CrearAgente()
    {
        try
        {
            if (contadorAgentes < AforoMaximo)
            {
                GameObject clon = Instantiate(persona, puntoInicio.transform.position, puntoInicio.transform.rotation);
                clon.GetComponentInChildren<Camino>().enabled = true;
                //clon.GetComponentInChildren<ParticleSystem>().Play();

                var probCovid = Random.Range(0, 100);
                if (probCovid > 80)
                {
                    aTieneCovid = true;
                    //clon.gameObject.SetActive(aTieneCovid);
                    clon.GetComponentInChildren<ParticleSystem>().Play(true);
                }
                else
                {
                    //clon.gameObject.SetActive(aTieneCovid);
                    clon.GetComponentInChildren<ParticleSystem>().Stop(true);
                }
                Debug.Log("CrearAgente " + contadorAgentes);
                contadorAgentes++;
                Invoke("CrearAgente", 2.0f);
            }
        }
        catch (System.Exception ex)
        {

            Debug.Log(ex.Message);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("initParticula"))
        {
            Debug.Log(other.gameObject + "Particula en el ambiente ");
        }
     
    }


}
