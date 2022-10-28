using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject destinoFinal;
    //public Camera[] camaras;

    // Start is called before the first frame update
    void Start()
    {
        agent.destination = destinoFinal.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //agent.destination = destinoFinal.transform.position;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycast;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycast))
            {
                agent.destination = raycast.point;
            }
        }
    }
}
