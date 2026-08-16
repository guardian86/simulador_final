using System.Linq;
using UnityEngine;

public class Agente : MonoBehaviour
{
    // Referencia al sistema de partículas propio para saber si está infectado
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        // Toma una instantánea del estado actual de infección de todos los agentes al inicio
        Invoke(nameof(SnapshotEstadoInicialAgentes), 2f);
    }

    private void SnapshotEstadoInicialAgentes()
    {
        var listaAgentes = GameObject.FindGameObjectsWithTag("tagPersonas");
        Globales.agenteCovid19.Clear();
        int idx = 0;
        foreach (var go in listaAgentes)
        {
            var p = go.GetComponentInChildren<ParticleSystem>();
            bool infectado = p != null && p.isEmitting;
            Globales.agenteCovid19.Add(new AgentesContagiados
            {
                cantidadContagiados = idx,
                tieneCovid = infectado
            });
            idx++;
        }
    }

    public class AgentesContagiados
    {
        public bool tieneCovid { get; set; }
        public int cantidadContagiados { get; set; }
    }
}
