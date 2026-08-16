using System.Linq;
using UnityEngine;

public class Particula : MonoBehaviour
{
    public float radioInfluenciaAerosol = 2.5f;
    public float tasaEmisionAerosol = 0.45f;
    public float factorAtenuacionAmbiente = 0.9f;

    private ParticleSystem miParticula;
    private Transform raizPropia; // agente dueño
    private EstadoSaludAgente estadoPropio;
    private Administrador admin;

    private void Awake()
    {
        miParticula = GetComponentInChildren<ParticleSystem>();
        // FIX: antes se usaba transform.root, que en este prefab es el objeto
        // contenedor ("Personaje (1)") que NUNCA se mueve — el NavMeshAgent real
        // mueve al hijo ("Capsule"), donde también vive este mismo script y
        // EstadoSaludAgente. Usar GetComponentInParent es robusto sin importar
        // en qué nivel de la jerarquía quede el script de salud.
        estadoPropio = GetComponentInParent<EstadoSaludAgente>();
        raizPropia = estadoPropio != null ? estadoPropio.transform : transform.root;
        admin = FindObjectOfType<Administrador>();
    }

    private Administrador ObtenerAdministrador()
    {
        if (admin == null)
            admin = FindObjectOfType<Administrador>();

        return admin;
    }

    private bool EstaInfectadoPropio()
    {
        return estadoPropio != null ? estadoPropio.estaInfectado : miParticula != null && miParticula.isEmitting;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Colisión con muros (para depurar)
        if (other.gameObject.tag == "muros")
        {
            if (EstaInfectadoPropio())
                Debug.Log($"Partícula de {raizPropia.name} choca con muro {other.gameObject.name}");
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!EstaInfectadoPropio())
            return;

        // FIX: mismo problema que en Awake — buscar el EstadoSaludAgente del OTRO
        // agente por jerarquía (GetComponentInParent) en vez de asumir que vive en
        // transform.root, y comparar por componente en vez de por Transform raíz.
        var estadoOtro = other.GetComponentInParent<EstadoSaludAgente>();
        if (estadoOtro == null || estadoOtro == estadoPropio || estadoOtro.estaInfectado)
            return;

        Transform rootOtro = estadoOtro.transform;
        float distancia = Vector3.Distance(raizPropia.position, rootOtro.position);
        if (distancia > radioInfluenciaAerosol)
            return;

        var administrador = ObtenerAdministrador();
        float factorDistancia = Mathf.Clamp01(1f - (distancia / Mathf.Max(0.1f, radioInfluenciaAerosol)));
        float factorMascarilla = administrador != null ? administrador.ObtenerFactorMascarillaAerosoles() : 1f;
        float factorVentilacion = administrador != null ? administrador.ObtenerFactorVentilacionAerosoles() : 1f;
        float dosis = tasaEmisionAerosol
            * factorDistancia
            * Mathf.Max(0.3f, factorAtenuacionAmbiente)
            * factorMascarilla
            * factorVentilacion
            * Time.deltaTime;

        estadoOtro.RegistrarExposicionAerosol(dosis, Time.deltaTime);
    }
}
