using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observador : MonoBehaviour
{
    public GameObject persona;
    public GameObject puntoInicio;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            crear();
        }

    }

    void crear()
    {
        Instantiate(persona, puntoInicio.transform);
        Invoke("crear", 2.0f);
    }
}
