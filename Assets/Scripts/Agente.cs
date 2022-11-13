using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agente : MonoBehaviour
{
    public GameObject particle;
    int secuencia = 0;
    private void Start()
    {
        
    }

    private void Update()
    {
        //OnCollisionEnter(particle);
        OnParticleCollision(particle);
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particula colisiona " + secuencia);
        secuencia++;
        if (particle.gameObject.tag.Equals("initParticula"))
        {
            Debug.Log(other.gameObject + "Particula en el ambiente ");
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
