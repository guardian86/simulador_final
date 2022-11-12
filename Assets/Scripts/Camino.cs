using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

public class Camino : MonoBehaviour
{
    int velocidadInit = 4;
    public int veloMax;
    // Start is called before the first frame update
    void Start()
    {

        this.GetComponent<NavMeshAgent>().speed = Random.Range(velocidadInit, velocidadInit + veloMax);
        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("meta");
        int salidaEscogida = Random.Range(0, listaSalidas.Length);

        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        this.GetComponent<NavMeshAgent>().SetDestination(v);

    }

    // Update is called once per frame
    void Update()
    {
    }

  

    void SalirCentroComercial()
    {
        this.GetComponent<NavMeshAgent>().speed = Random.Range(velocidadInit, velocidadInit + veloMax);
        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("salida_cc");
        int salidaEscogida = Random.Range(0, listaSalidas.Length);

        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        this.GetComponent<NavMeshAgent>().SetDestination(v);
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("salida_cc"))
        {
            Debug.Log(other.gameObject);
            Destroy(this.transform.parent.gameObject);
        }
        if (other.gameObject.tag.Equals("meta"))
        {
            Debug.Log(other.gameObject);
            Invoke("SalirCentroComercial", Random.Range(7f,15f));
        }
    }

}
