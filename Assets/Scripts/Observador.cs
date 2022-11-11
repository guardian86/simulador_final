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
        /*if (Input.GetKeyUp(KeyCode.Space))
        {
            CrearAgente();
        }*/
        

    }

    void CrearAgente()
    {

        if (contadorAgentes < AforoMaximo)
        {
            GameObject clon = Instantiate(persona, puntoInicio.transform.position, puntoInicio.transform.rotation);
            clon.GetComponentInChildren<Camino>().enabled = true;
            clon.GetComponentInChildren<Salir>().enabled = true;
            Debug.Log("CrearAgente " + contadorAgentes);
            contadorAgentes++;
            Invoke("CrearAgente", 2.0f);
        }

        

    }




}
