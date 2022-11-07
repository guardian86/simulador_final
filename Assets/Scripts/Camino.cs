using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Camino : MonoBehaviour
{
    public GameObject salidas;
    //int localesIngresados = 0;
    // Start is called before the first frame update
    void Start()
    {

        this.GetComponent<NavMeshAgent>().speed = Random.Range(0, 8);
        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("meta");
        int salidaEscogida = Random.Range(0, listaSalidas.Length);

        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        this.GetComponent<NavMeshAgent>().SetDestination(v);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //Invoke("Salir", 10.0f);
            Salir();
        }
    }

    void Salir()
    {
        this.GetComponent<NavMeshAgent>().speed = Random.Range(0, 8);
        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("salida_cc");
        int salidaEscogida = Random.Range(0, listaSalidas.Length);

        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        bool v1 = this.GetComponent<NavMeshAgent>().SetDestination(v);

        var clones = GameObject.FindGameObjectsWithTag("salida_cc");
        if (clones.Any())
        {
            foreach (var clone in clones)
            {
                Destroy(clone);
            }
        }


    }

}
