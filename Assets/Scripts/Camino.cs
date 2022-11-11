using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

public class Camino : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        this.GetComponent<NavMeshAgent>().speed = 8; //Random.Range(0, 8);
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
        this.GetComponent<NavMeshAgent>().speed = 8; //Random.Range(0, 8);
        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("salida_cc");
        int salidaEscogida = Random.Range(0, listaSalidas.Length);

        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        this.GetComponent<NavMeshAgent>().SetDestination(v);



        //DestroyNearest(listaSalidas.);

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
            Invoke("SalirCentroComercial", Random.Range(5f,10f));
        }
    }


    //public void DestroyNearest(List<Item> items)
    //{
    //    // Find nearest item.
    //    Item nearest = null;
    //    float distance = 0;

    //    for (int i = 0; i < items.Count; i++)
    //    {
    //        float tempDistance = Vector3.Distance(transform.position, items[i].position);
    //        if (nearest == null || tempDistance < distance)
    //        {
    //            nearest = items[i];
    //            distance = tempDistance;
    //        }
    //    }

    //    // Remove from list.
    //    items.Remove(nearest);

    //    // Destroy object.
    //    Destroy(nearest.gameObject);
    //}


}
