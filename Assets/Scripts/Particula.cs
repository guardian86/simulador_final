using System.Linq;
using UnityEngine;

public class Particula : MonoBehaviour
{
    // Probabilidad de contagio al contacto (0..1)
    [Range(0f, 1f)] public float probContagio = 0.25f;
    // Tiempo de enfriamiento para no contagiar al mismo agente repetidamente
    public float cooldownRecontagioSeg = 3f;

    private ParticleSystem miParticula;
    private Transform raizPropia; // agente dueño

    private void Awake()
    {
        miParticula = GetComponentInChildren<ParticleSystem>();
        raizPropia = transform.root;
    }

    private bool EstaInfectadoPropio()
    {
        return miParticula != null && miParticula.isEmitting;
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

        // Intento de contagio por proximidad entre personas
    if (other.gameObject.tag != "tagPersonas") return;

        // Evitar auto-colisión del mismo agente
        if (other.transform.root == raizPropia) return;

        // Solo contagia si el dueño actual está infectado (emitiendo)
        if (!EstaInfectadoPropio()) return;

        // Buscar el sistema de partículas del otro agente
        var psOtro = other.GetComponentInChildren<ParticleSystem>();
        if (psOtro == null) return;

        // Si ya está infectado, no hacemos nada
        if (psOtro.isEmitting) return;

        // Enfriamiento por-agente para este otro específico (opcional, basado en un flag temporal)
        var marcador = other.GetComponent<UltimoContacto>();
        if (marcador == null) marcador = other.gameObject.AddComponent<UltimoContacto>();
        if (Time.time - marcador.ultimoContactoTime < cooldownRecontagioSeg) return;
        marcador.ultimoContactoTime = Time.time;

        // Probabilidad de contagio
        var rnd = Random.Range(0f, 1f);
        if (rnd <= probContagio)
        {
            psOtro.Play(true); // Contagiado!

            // Registrar en memoria global (simple)
            Globales.agenteCovid19.Add(new Agente.AgentesContagiados
            {
                tieneCovid = true,
                cantidadContagiados = 1
            });

            Debug.Log($"Contagio: {raizPropia.name} -> {other.transform.root.name} (p={probContagio:P0})");
        }
    }

    // Componente helper minimalista para cooldown de contacto por-agente
    private class UltimoContacto : MonoBehaviour
    {
        public float ultimoContactoTime = -999f;
    }
}
