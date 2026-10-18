using UnityEngine;

[CreateAssetMenu(menuName = "Simulador/Config", fileName = "SimuladorConfig")]
public class SimuladorConfig : ScriptableObject
{
    [Header("Agentes")]
    public int aforoMaximo = 30;
    public float intervaloSpawn = 2f;
    public bool autoSpawn = true;

    [Header("Modelo aerosol")]
    [Range(0f,1f)] public float probabilidadIngresoInfectado = 0.2f;
    public float umbralContagioAerosolBase = 1.2f;
    [Range(0f,12f)] public float nivelVentilacionACH = 6f;
    [Range(0f,100f)] public float eficaciaMascarillaPorcentaje = 0f;

    [Header("Perfil de virus (por defecto: SARS-CoV-2)")]
    public string nombreVirus = "SARS-CoV-2 (COVID-19)";
    public float tasaEmisionAerosol = 0.45f;
    public float radioInfluenciaAerosol = 2.5f;
    [Range(0.3f,1f)] public float factorAtenuacionAmbiente = 0.9f;

    [Header("Lotes")]
    public int cantidadSimulacionesLote = 5;
    public float duracionSimulacionSegundos = 45f;
    public int aforoMinimoPorSimulacion = 20;
    public int aforoMaximoPorSimulacion = 60;

    [Header("Reportes")]
    public bool usarRutaPortable = true;
}
