using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Camino : MonoBehaviour
{
    public GameObject salidas;
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
       
    }
}
