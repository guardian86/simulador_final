using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using static UnityEngine.ParticleSystem;


[Serializable]
public class Observador : MonoBehaviour
{
    EmissionModule emision;
    private string json;
    int contAgentesCovid = 0;

    // Update is called once per frame
    private void Start()
    {
        Invoke("ObtenerAgentes", 20f);
    }

    void Update()
    {


    }



    private void ObtenerAgentes()
    {
        GameObject[] listAgentes = GameObject.FindGameObjectsWithTag("Agentes");
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
                Observador x = CreateFromJSON(json);

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


    public static Observador CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Observador>(jsonString);
    }



}
