using UnityEngine;

[CreateAssetMenu(menuName = "Simulador/Config", fileName = "SimuladorConfig")]
public class SimuladorConfig : ScriptableObject
{
    [Header("Agentes")]
    public int aforoMaximo = 30;
    public float intervaloSpawn = 2f;
    public bool autoSpawn = true;

    [Header("Contagio")]
    [Range(0f,1f)] public float probContagio = 0.25f;

    [Header("Reportes")]
    public bool usarRutaPortable = true;
}
