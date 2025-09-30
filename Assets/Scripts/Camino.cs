using UnityEngine;
using UnityEngine.AI;

public class Camino : MonoBehaviour
{
    int velocidadInit = 4;
    public int veloMax;
    Administrador administrador;
    //bool generarRpe = true;


    // Start is called before the first frame update
    void Start()
    {

        this.GetComponent<NavMeshAgent>().speed = Random.Range(velocidadInit, velocidadInit + veloMax);
        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("meta");
        int salidaEscogida = Random.Range(0, listaSalidas.Length);

        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        this.GetComponent<NavMeshAgent>().SetDestination(v);

    }

    // Update is called once per frame
    void Update()
    {
        //if (generarRpe)
        //{
        //    Invoke("InvocarRptAgentes", 5f);
        //    generarRpe= false;
        //}
    }

    private void InvocarRptAgentes()
    {
        if (administrador == null)
            administrador = FindObjectOfType<Administrador>();
        if (administrador != null)
            administrador.ObtenerReporteAgentes();
        else
            Debug.LogWarning("Administrador no encontrado en la escena para generar reporte");
    }

    void SalirCentroComercial()
    {

        bool irNuevoLocal = false;
        this.GetComponent<NavMeshAgent>().speed = Random.Range(velocidadInit, velocidadInit + veloMax);

        irNuevoLocal = Random.Range(0, 3) > 1 ? true : false;
        if (irNuevoLocal) Invoke("Start", 1f);

        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("salida_cc");
        int salidaEscogida = Random.Range(0, listaSalidas.Length);

        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        this.GetComponent<NavMeshAgent>().SetDestination(v);

        // Opción: generar un reporte una sola vez, cuando empiece a salir gente
        if (Globales.generaRpt)
        {
            InvocarRptAgentes();
            Globales.generaRpt = false;
        }


        //administrador.ObtenerReporteAgentes();

    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("salida_cc"))
        {
            //Debug.Log(other.gameObject);
            if (other.gameObject)
            {
                Destroy(this.transform.parent.gameObject);
            }
        }
        if (other.gameObject.tag.Equals("meta"))
        {

            //Debug.Log(other.gameObject);
            Invoke("SalirCentroComercial", Random.Range(7f, 15f));
        }
    }


}
