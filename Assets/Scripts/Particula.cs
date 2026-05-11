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

    private void Awake()
    {
        miParticula = GetComponentInChildren<ParticleSystem>();
        raizPropia = transform.root;
        estadoPropio = raizPropia.GetComponent<EstadoSaludAgente>();
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

        var rootOtro = other.transform.root;
        bool esPersona = (other.gameObject.tag == "tagPersonas") || (rootOtro != null && rootOtro.gameObject.tag == "tagPersonas");
        if (!esPersona || rootOtro == raizPropia)
            return;

        var estadoOtro = rootOtro != null ? rootOtro.GetComponent<EstadoSaludAgente>() : null;
        if (estadoOtro == null || estadoOtro.estaInfectado)
            return;

        float distancia = Vector3.Distance(raizPropia.position, rootOtro.position);
        if (distancia > radioInfluenciaAerosol)
            return;

        float factorDistancia = Mathf.Clamp01(1f - (distancia / Mathf.Max(0.1f, radioInfluenciaAerosol)));
        float dosis = tasaEmisionAerosol * factorDistancia * Mathf.Max(0.3f, factorAtenuacionAmbiente) * Time.deltaTime;
        estadoOtro.RegistrarExposicionAerosol(dosis, Time.deltaTime);
    }
}
