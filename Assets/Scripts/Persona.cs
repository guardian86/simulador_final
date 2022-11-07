using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persona : MonoBehaviour
{
    
    public GameObject COVID19;

    // Start is called before the first frame update
    void Start()
    {
        COVID19.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Agente contagiado de Covid");
        COVID19.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Agente sin Covid");
        COVID19.SetActive(false);
    }


}
