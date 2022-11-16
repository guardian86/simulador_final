using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particula : MonoBehaviour
{
    //public ParticleSystem particle;
    int secuencia = 0;
    private void Start()
    {
        
    }

    private void Update()
    {
        //OnCollisionEnter(particle);
        //OnParticleCollision(particle);
    }


    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("XXXXX");
        //initParticula
   
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("YYYYY");
        if (other.gameObject.tag.Equals("muros"))
        {
            Debug.Log("Particula choca con Muros  " + secuencia + " " + other.gameObject.name);
            secuencia++;
        }

        if (other.gameObject.tag.Equals("tagPersonas"))
        {
            Debug.Log("Particula choca con Agente " + secuencia + " " + other.gameObject.name);
            secuencia++;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Particula colisiona " + secuencia);
    //    secuencia++;
    //    if (particle.gameObject.tag.Equals("initParticula"))
    //    {
    //        Debug.Log(collision.gameObject + "Particula en el ambiente ");
    //    }
    //}

}
