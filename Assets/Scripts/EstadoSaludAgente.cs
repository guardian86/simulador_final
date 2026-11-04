using UnityEngine;

public class EstadoSaludAgente : MonoBehaviour
{
    public bool fueCasoInicial { get; private set; }
    public bool fueContagioSecundario { get; private set; }
    public bool estaInfectado { get; private set; }
    public float dosisAcumulada { get; private set; }
    public float tiempoExposicionAcumulado { get; private set; }
    public float umbralContagioActual { get; private set; }

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

    public void RegistrarExposicionAerosol(float dosis, float deltaTiempo)
    {
        if (estaInfectado || dosis <= 0f)
            return;

        dosisAcumulada += dosis;
        tiempoExposicionAcumulado += Mathf.Max(0f, deltaTiempo);

        if (dosisAcumulada >= umbralContagioActual)
            InfectarPorExposicion();
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
