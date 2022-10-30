using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Particula : MonoBehaviour
{
    int salidaParticulas = 1;
    public GameObject objeto;
    public ParticleSystem particula;
    public List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        particula = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();

    }

    // Update is called once per frame
    void Update()
    {
        OnParticleCollision(objeto);
    }

    public void OnParticleCollision(GameObject other)
    {
        salidaParticulas += 1;
        Debug.Log(salidaParticulas);

        //int numCollisionEvents = particula.GetCollisionEvents(other, collisionEvents);

        //Rigidbody rb = other.GetComponent<Rigidbody>();
        //int i = 0;

        //Debug.Log("Colision de la particula contra un objeto ");

        //while (i < numCollisionEvents)
        //{
        //    if (rb)
        //    {
        //        Vector3 pos = collisionEvents[i].intersection;
        //        Vector3 force = collisionEvents[i].velocity * 10;
        //        rb.AddForce(force);
        //    }

        //    i++;
        //}
    }
}
