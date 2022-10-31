using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambioCamaras : MonoBehaviour
{
    public GameObject[] listacamaras;
    int ncamaras = 4;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < ncamaras; i++)
        {
            listacamaras[i].gameObject.SetActive(false);
        }
        listacamaras[0].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            ApagarCamaras();
            listacamaras[0].gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            ApagarCamaras();
            listacamaras[1].gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            ApagarCamaras();
            listacamaras[2].gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            ApagarCamaras();
            listacamaras[3].gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Alpha5))
        {
            ApagarCamaras();
            listacamaras[4].gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Alpha6))
        {
            ApagarCamaras();
            listacamaras[5].gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Alpha7))
        {
            ApagarCamaras();
            listacamaras[6].gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Alpha8))
        {
            ApagarCamaras();
            listacamaras[7].gameObject.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Alpha9))
        {
            ApagarCamaras();
            listacamaras[8].gameObject.SetActive(true);
        }

       void ApagarCamaras()
        {
            for (int i = 0; i < ncamaras; i++)
            {
                listacamaras[i].gameObject.SetActive(false);
            }
        }
    }
}
