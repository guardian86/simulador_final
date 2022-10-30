using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EliminarPersonas : MonoBehaviour
{
    public GameObject personasEliminar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Application.Quit();
            //Debug.Log("Escape pressed!");
            var clones = GameObject.FindGameObjectsWithTag("tagPersonas");
            if (clones.Any())
            {
                foreach (var clone in clones)
                {
                    Destroy(clone);
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }


}
