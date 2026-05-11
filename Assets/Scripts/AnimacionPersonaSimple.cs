using UnityEngine;
using UnityEngine.AI;

public class AnimacionPersonaSimple : MonoBehaviour
{
    public Transform brazoIzquierdo;
    public Transform brazoDerecho;
    public Transform antebrazoIzquierdo;
    public Transform antebrazoDerecho;
    public Transform piernaIzquierda;
    public Transform piernaDerecha;
    public Transform visualRaiz;

    public float amplitudBrazos = 28f;
    public float amplitudPiernas = 24f;
    public float velocidadBase = 7f;
    public float reboteVertical = 0.03f;

    private NavMeshAgent agenteNavMesh;
    private Vector3 posicionLocalInicialVisual;
    private Quaternion rotBrazoIzquierdo;
    private Quaternion rotBrazoDerecho;
    private Quaternion rotAntebrazoIzquierdo;
    private Quaternion rotAntebrazoDerecho;
    private Quaternion rotPiernaIzquierda;
    private Quaternion rotPiernaDerecha;
    private float fase;

    private void Awake()
    {
        agenteNavMesh = GetComponent<NavMeshAgent>();
        if (visualRaiz != null)
            posicionLocalInicialVisual = visualRaiz.localPosition;

        if (brazoIzquierdo != null) rotBrazoIzquierdo = brazoIzquierdo.localRotation;
        if (brazoDerecho != null) rotBrazoDerecho = brazoDerecho.localRotation;
        if (antebrazoIzquierdo != null) rotAntebrazoIzquierdo = antebrazoIzquierdo.localRotation;
        if (antebrazoDerecho != null) rotAntebrazoDerecho = antebrazoDerecho.localRotation;
        if (piernaIzquierda != null) rotPiernaIzquierda = piernaIzquierda.localRotation;
        if (piernaDerecha != null) rotPiernaDerecha = piernaDerecha.localRotation;
    }

    private void Update()
    {
        float velocidad = agenteNavMesh != null ? agenteNavMesh.velocity.magnitude : 0f;
        float factorMovimiento = Mathf.Clamp01(velocidad / 2.5f);
        fase += Time.deltaTime * velocidadBase * Mathf.Lerp(0.35f, 1.3f, factorMovimiento);

        float seno = Mathf.Sin(fase);
        float senoSecundario = Mathf.Sin(fase + 0.6f);

        AplicarRotacion(brazoIzquierdo, rotBrazoIzquierdo, seno * amplitudBrazos * factorMovimiento);
        AplicarRotacion(brazoDerecho, rotBrazoDerecho, -seno * amplitudBrazos * factorMovimiento);
        AplicarRotacion(antebrazoIzquierdo, rotAntebrazoIzquierdo, senoSecundario * amplitudBrazos * 0.35f * factorMovimiento);
        AplicarRotacion(antebrazoDerecho, rotAntebrazoDerecho, -senoSecundario * amplitudBrazos * 0.35f * factorMovimiento);
        AplicarRotacion(piernaIzquierda, rotPiernaIzquierda, -seno * amplitudPiernas * factorMovimiento);
        AplicarRotacion(piernaDerecha, rotPiernaDerecha, seno * amplitudPiernas * factorMovimiento);

        if (visualRaiz != null)
        {
            float rebote = Mathf.Abs(seno) * reboteVertical * factorMovimiento;
            visualRaiz.localPosition = posicionLocalInicialVisual + new Vector3(0f, rebote, 0f);
        }
    }

    private void AplicarRotacion(Transform objetivo, Quaternion baseRotacion, float anguloX)
    {
        if (objetivo == null)
            return;

        objetivo.localRotation = baseRotacion * Quaternion.Euler(anguloX, 0f, 0f);
    }
}
