using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persona : MonoBehaviour
{
    //private bool tieneCovid = false;
    public GameObject persona;
    public GameObject puntoInicio;
    //public GameObject particula;
    //private float quitarPersonas = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
//            Instantiate(persona, puntoInicio.transform);
            //realizar el random para generar la persona con covid o sin covid 
            //switch (Random.Range(0, 4))
            //{
            //    case 0:
            //        Instantiate(persona, transform.position, Quaternion.identity);
            //        break;
            //    case 1:
            //        Instantiate(persona, transform.position, Quaternion.identity);
            //        break;
            //    case 2:
            //        Instantiate(persona, transform.position, Quaternion.identity);
            //        break;
            //    case 3:
            //        Instantiate(persona, transform.position, Quaternion.identity);
            //        break;
            //    case 4:
            //        Instantiate(persona, transform.position, Quaternion.identity);
            //        break;
            //    default:
            //        break;
            //}

        }
    }


}
