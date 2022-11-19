using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observador : MonoBehaviour
{
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
       
    }

    void CrearAgente()
    {
        try
        {
            if (contadorAgentes < AforoMaximo)
            {
                GameObject clon = Instantiate(persona, puntoInicio.transform.position, puntoInicio.transform.rotation);
                clon.GetComponentInChildren<Camino>().enabled = true;
                clon.GetComponentInChildren<Particula>().enabled = true;
                clon.gameObject.SetActive(true);
                //clon.GetComponentInChildren<ParticleSystem>().Play(true);
                var probCovid = Random.Range(0, 100);

                if (probCovid > 80) clon.GetComponentInChildren<ParticleSystem>().Play(true);
                else clon.GetComponentInChildren<ParticleSystem>().Stop(true);
                
                Debug.Log("CrearAgente " + contadorAgentes);
                contadorAgentes++;
                Invoke("CrearAgente", 2.0f);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message.ToString());
        }
    }




}
