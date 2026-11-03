using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Camino : MonoBehaviour
{
    int velocidadInit = 4;
    public int veloMax;
    Administrador administrador;
    private NavMeshAgent agenteNavMesh;
    //bool generarRpe = true;

    private void Awake()
    {
        agenteNavMesh = GetComponent<NavMeshAgent>();
    }


    // Start is called before the first frame update
    void Start()
    {
        ReiniciarRuta();

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
        // Grupo cerrado (homologación con el Aula): el agente no abandona el recinto,
        // elige otro destino y sigue circulando.
        if (administrador == null)
            administrador = FindObjectOfType<Administrador>();
        if (administrador != null && administrador.modoGrupoFijo)
        {
            ReiniciarRuta();
            return;
        }

        if (!PrepararAgenteNavMesh())
            return;

        bool irNuevoLocal = false;
        agenteNavMesh.speed = Random.Range(velocidadInit, velocidadInit + veloMax);

        irNuevoLocal = Random.Range(0, 3) > 1 ? true : false;
        if (irNuevoLocal) Invoke(nameof(ReiniciarRuta), 1f);

        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("salida_cc");
        if (listaSalidas == null || listaSalidas.Length == 0)
            return;

        int salidaEscogida = Random.Range(0, listaSalidas.Length);

        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        agenteNavMesh.SetDestination(v);

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
            // Grupo cerrado: nadie sale del recinto (evita perder al caso índice).
            var adminGrupo = FindObjectOfType<Administrador>();
            if (adminGrupo != null && adminGrupo.modoGrupoFijo)
                return;
            //Debug.Log(other.gameObject);
            if (other.gameObject)
            {
                var admin = FindObjectOfType<Administrador>();
                if (admin != null)
                {
                    admin.ReleaseAgente(this.transform.root.gameObject);
                }
                else
                {
                    Destroy(this.transform.root.gameObject);
                }
            }
        }
        if (other.gameObject.tag.Equals("meta"))
        {

            //Debug.Log(other.gameObject);
            CancelInvoke(nameof(SalirCentroComercial));
            Invoke(nameof(SalirCentroComercial), Random.Range(7f, 15f));
        }
    }

    public void ReiniciarRuta()
    {
        if (!PrepararAgenteNavMesh())
            return;

        agenteNavMesh.speed = Random.Range(velocidadInit, velocidadInit + veloMax);
        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("meta");
        if (listaSalidas == null || listaSalidas.Length == 0) return;
        listaSalidas = ElegirMetasCandidatas(listaSalidas);
        int salidaEscogida = Random.Range(0, listaSalidas.Length);
        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        agenteNavMesh.SetDestination(v);
    }

    // En grupo cerrado se limita la zona de destinos y se evita elegir el destino donde el agente ya está.
    private GameObject[] ElegirMetasCandidatas(GameObject[] metas)
    {
        if (administrador == null)
            administrador = FindObjectOfType<Administrador>();
        if (administrador == null || !administrador.modoGrupoFijo)
            return metas;

        var zona = administrador.ObtenerMetasZona(metas);
        if (zona.Length > 1)
        {
            var otras = zona.Where(m => Vector3.Distance(m.transform.position, transform.position) > 2f).ToArray();
            if (otras.Length > 0)
                return otras;
        }
        return zona;
    }

    private bool PrepararAgenteNavMesh()
    {
        if (agenteNavMesh == null)
            agenteNavMesh = GetComponent<NavMeshAgent>();

        if (agenteNavMesh == null || !agenteNavMesh.enabled || !gameObject.activeInHierarchy)
            return false;

        if (agenteNavMesh.isOnNavMesh)
            return true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            agenteNavMesh.Warp(hit.position);
            return agenteNavMesh.isOnNavMesh;
        }

        Debug.LogWarning($"No se encontró NavMesh cercano para {name}; se omite SetDestination.");
        return false;
    }


}
