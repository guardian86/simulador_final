using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particula : MonoBehaviour
{
    private bool aTieneCovid = false;
    int salidaParticulas = 1;
    public GameObject escenario;
    public ParticleSystem particulas;
    private System.DateTime tVidaParti;

    // Start is called before the first frame update
    void Start()
    {
        var probCovid = Random.Range(0, 100);

        if (probCovid > 80)
        {
            aTieneCovid = true;
            particulas.Play(aTieneCovid);
        }
        else
        {
            particulas.Play(aTieneCovid);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //OnParticleCollision(escenario);
    }

    public void OnParticleCollision(GameObject other)
    {
        tVidaParti = System.DateTime.Now;
        salidaParticulas += 1;
        Debug.Log("Particula en Colision " + salidaParticulas + tVidaParti );

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
