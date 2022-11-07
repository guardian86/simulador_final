using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Salida : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            this.GetComponent<NavMeshAgent>().speed = Random.Range(0, 8);
            GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("salida_cc");
            int salidaEscogida = Random.Range(0, listaSalidas.Length);

            Vector3 v = listaSalidas[salidaEscogida].transform.position;
            this.GetComponent<NavMeshAgent>().SetDestination(v);

            Invoke("SalirCentroComercial", 20.0f);
           
        }
    }


    void SalirCentroComercial()
    {
        var clones = GameObject.FindGameObjectsWithTag("tagPersonas");
        if (clones.Any())
        {
            foreach (var clone in clones)
            {
                Destroy(clone);
            }
        }
    }

}
