using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particula : MonoBehaviour
{
    private bool aTieneCovid = false;
    int salidaParticulas = 1;
    public List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();

    }

    // Update is called once per frame
    void Update()
    {
        var probCovid = Random.Range(0, 100);
        if (probCovid > 80)
        {
            aTieneCovid = true;
            this.GetComponent<ParticleSystem>().Play(aTieneCovid);
        }
        else
        {
            this.GetComponent<ParticleSystem>().Play(aTieneCovid);
        }
    }

    public void OnParticleCollision(GameObject other)
    {
        salidaParticulas += 1;
        Debug.Log("Particula en Colision " + salidaParticulas);

        //int numCollisionEvents = particula.GetCollisionEvents(other, collisionEvents);

        //Rigidbody rb = other.GetComponent<Rigidbody>();
        //int i = 0;

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
