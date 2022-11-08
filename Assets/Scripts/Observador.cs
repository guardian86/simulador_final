using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observador : MonoBehaviour
{
    public GameObject persona;
    public GameObject puntoInicio;
    public int AforoMaximo;
    int contadorAgentes = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CrearAgente();
        }

    }

    void CrearAgente()
    {

        if (contadorAgentes <= AforoMaximo)
        {
            Instantiate(persona, puntoInicio.transform);
            contadorAgentes++;
            Invoke("CrearAgente", 2.0f);
        }
        
    }




}
