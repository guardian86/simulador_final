using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Particula : MonoBehaviour
{
    //public ParticleSystem particle;
    private GameObject[] listAgentes;
    int contadorMuros = 0;
    int contadorPersonas = 0;

    private void Start()
    {
       
    }

    private void Update()
    {

        //Invoke("ActivarDispararParticula", 7f);
        
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
        listAgentes = GameObject.FindGameObjectsWithTag("tagPersonas");
        
        if (other.gameObject.tag.Equals("muros"))
        {
            contadorMuros = listAgentes.Count() - 1;
            if (listAgentes[contadorMuros].GetComponentInChildren<ParticleSystem>().isEmitting)
            {
                Debug.Log("Particula choca con Muro  " + contadorMuros + " " + other.gameObject.name);
            }
        }

        if (other.gameObject.tag.Equals("tagPersonas"))
        {
            contadorPersonas = listAgentes.Count() - 1;
            if (listAgentes[contadorPersonas].GetComponentInChildren<ParticleSystem>().isEmitting)
            {
                Debug.Log("Particula choca con Agente " + contadorPersonas + " " + other.gameObject.name);
            }
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
