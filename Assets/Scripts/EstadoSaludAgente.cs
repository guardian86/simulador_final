using UnityEngine;

public class EstadoSaludAgente : MonoBehaviour
{
    public bool fueCasoInicial { get; private set; }
    public bool fueContagioSecundario { get; private set; }
    public bool estaInfectado { get; private set; }
    public float dosisAcumulada { get; private set; }
    public float tiempoExposicionAcumulado { get; private set; }
    public float umbralContagioActual { get; private set; }
    // Trazabilidad del recorrido (tiempo simulado): ingreso al recinto y locales visitados.
    public float tiempoIngreso { get; set; }
    public int visitas { get; set; }

    private ParticleSystem particulas;

    private void Awake()
    {
        particulas = GetComponentInChildren<ParticleSystem>();
    }

    public void ConfigurarEstadoInicial(bool infectadoInicial, float umbralContagioBase)
    {
        if (particulas == null)
            particulas = GetComponentInChildren<ParticleSystem>();

        dosisAcumulada = 0f;
        tiempoExposicionAcumulado = 0f;
        fueCasoInicial = infectadoInicial;
        tiempoIngreso = Time.time;
        visitas = 0;
        fueContagioSecundario = false;
        umbralContagioActual = Mathf.Max(0.15f, umbralContagioBase * Random.Range(0.9f, 1.1f));
        estaInfectado = infectadoInicial;

        if (particulas != null)
        {
            if (infectadoInicial)
                particulas.Play(true);
            else
                particulas.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    // Dosis-respuesta exponencial (Lelieveld et al., 2020): la probabilidad de contagio por una dosis
    // d es 1 - e^(-d/k), con k = dosis infecciosa característica (umbralContagioAerosolBase).
    // Aplicada por cuadro, la probabilidad acumulada al final de la visita es 1 - e^(-D/k), con D la
    // dosis total. La dosis se sigue registrando después del contagio para poder calcular el riesgo esperado.
    public float dosisCaracteristica { get; set; } = 1.2f;
    public bool usarDosisRespuesta { get; set; } = true;

    public void RegistrarExposicionAerosol(float dosis, float deltaTiempo)
    {
        if (fueCasoInicial || dosis <= 0f)
            return;

        dosisAcumulada += dosis;
        tiempoExposicionAcumulado += Mathf.Max(0f, deltaTiempo);

        if (estaInfectado)
            return;

        if (usarDosisRespuesta)
        {
            float p = 1f - Mathf.Exp(-dosis / Mathf.Max(1e-6f, dosisCaracteristica));
            if (Random.value < p)
                InfectarPorExposicion();
        }
        else if (dosisAcumulada >= umbralContagioActual)
        {
            InfectarPorExposicion();
        }
    }

    public float RiesgoEsperado()
    {
        if (fueCasoInicial) return 0f;
        return 1f - Mathf.Exp(-dosisAcumulada / Mathf.Max(1e-6f, dosisCaracteristica));
    }

    private void InfectarPorExposicion()
    {
        estaInfectado = true;
        fueContagioSecundario = true;

        // El contagio secundario cambia a rojo (AspectoAgente) pero no emite la nube de aerosol:
        // está en período de latencia. Solo los casos índice muestran partículas.
        var admin = FindObjectOfType<Administrador>();
        if (particulas != null && admin != null && admin.secundariosContagian)
            particulas.Play(true);
    }
}
