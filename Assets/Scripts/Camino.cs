using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Camino : MonoBehaviour
{
    int velocidadInit = 4;
    public int veloMax;
    Administrador administrador;
    private NavMeshAgent agenteNavMesh;
    // En grupo cerrado: true mientras el agente camina hacia la salida, para que los destinos
    // que cruza en el camino no lo desvíen de nuevo hacia la zona de inicio.
    private bool caminoASalida;
    // Grupo cerrado: evita que el temblor físico del agente sobre el destino (entra/sale del trigger
    // decenas de veces por segundo) reinicie una y otra vez el temporizador de espera.
    private GameObject ultimaMeta;
    private bool salidaPendiente;
    public static int diagSalirLlamadas, diagEntradasMeta, diagSalidaTrigger, diagDesvios;
    public bool CaminoASalida { get { return caminoASalida; } }
    public bool EstaDetenido { get { return agenteNavMesh != null && agenteNavMesh.enabled && agenteNavMesh.isOnNavMesh && agenteNavMesh.velocity.sqrMagnitude < 0.01f; } }
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
        if (administrador == null)
            administrador = FindObjectOfType<Administrador>();

        if (!PrepararAgenteNavMesh())
            return;

        diagSalirLlamadas++;
        salidaPendiente = false;
        ultimaMeta = null;
        bool irNuevoLocal = false;
        agenteNavMesh.speed = Random.Range(velocidadInit, velocidadInit + veloMax);

        irNuevoLocal = Random.Range(0, 3) > 1 ? true : false;
        if (irNuevoLocal) { Invoke(nameof(ReiniciarRuta), 1f); diagDesvios++; }

        GameObject[] listaSalidas = GameObject.FindGameObjectsWithTag("salida_cc");
        if (listaSalidas == null || listaSalidas.Length == 0)
            return;

        int salidaEscogida = Random.Range(0, listaSalidas.Length);

        Vector3 v = listaSalidas[salidaEscogida].transform.position;
        agenteNavMesh.SetDestination(v);
        caminoASalida = true;

        // Opción: generar un reporte una sola vez, cuando empiece a salir gente
        if (Globales.generaRpt && !(administrador != null && administrador.modoGrupoFijo))
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
            diagSalidaTrigger++;
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
            diagEntradasMeta++;

            if (administrador == null)
                administrador = FindObjectOfType<Administrador>();
            if (administrador != null && administrador.modoGrupoFijo)
            {
                if (caminoASalida)
                    return;
                if (salidaPendiente && ultimaMeta == other.gameObject)
                    return;
                ultimaMeta = other.gameObject;
                salidaPendiente = true;
            }
            //Debug.Log(other.gameObject);
            CancelInvoke(nameof(SalirCentroComercial));
            Invoke(nameof(SalirCentroComercial), Random.Range(7f, 15f));
        }
    }

    public void ReiniciarRuta()
    {
        if (!PrepararAgenteNavMesh())
            return;

        caminoASalida = false;
        salidaPendiente = false;
        ultimaMeta = null;
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
