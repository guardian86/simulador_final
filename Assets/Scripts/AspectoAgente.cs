using UnityEngine;

public class AspectoAgente : MonoBehaviour
{
    public Renderer renderCuerpo;
    public Renderer renderCabeza;
    public Renderer[] rendersPiel;
    public Renderer[] rendersRopaPrincipal;
    public Renderer[] rendersRopaSecundaria;
    public Renderer[] rendersCabello;

    public Color colorPielSano = new Color(0.98f, 0.82f, 0.68f);
    public Color colorPielInfectado = new Color(0.96f, 0.56f, 0.56f);
    public Color colorRopaInfectado = new Color(0.72f, 0.22f, 0.22f);

    private readonly Color[] paletaRopaPrincipal =
    {
        new Color(0.18f, 0.45f, 0.85f),
        new Color(0.12f, 0.62f, 0.48f),
        new Color(0.82f, 0.53f, 0.17f),
        new Color(0.53f, 0.33f, 0.74f),
        new Color(0.16f, 0.17f, 0.24f)
    };

    private readonly Color[] paletaRopaSecundaria =
    {
        new Color(0.16f, 0.18f, 0.24f),
        new Color(0.36f, 0.36f, 0.4f),
        new Color(0.22f, 0.29f, 0.36f),
        new Color(0.48f, 0.41f, 0.28f)
    };

    private readonly Color[] paletaCabello =
    {
        new Color(0.12f, 0.09f, 0.06f),
        new Color(0.28f, 0.18f, 0.1f),
        new Color(0.62f, 0.49f, 0.28f),
        new Color(0.08f, 0.08f, 0.1f)
    };

    private ParticleSystem particulas;
    private Color colorRopaPrincipalSano;
    private Color colorRopaSecundariaSano;
    private Color colorCabelloSano;

    private void Awake()
    {
        particulas = GetComponentInChildren<ParticleSystem>();
        InicializarColoresBase();
        AplicarColores(false);
    }

    private void OnEnable()
    {
        CancelInvoke(nameof(ActualizarAspecto));
        InvokeRepeating(nameof(ActualizarAspecto), 0f, 0.3f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ActualizarAspecto));
    }

    private void ActualizarAspecto()
    {
        bool infectado = particulas != null && particulas.isEmitting;
        AplicarColores(infectado);
    }

    private void AplicarColores(bool infectado)
    {
        AplicarColor(rendersPiel, infectado ? colorPielInfectado : colorPielSano);
        AplicarColor(rendersRopaPrincipal, infectado ? Mezclar(colorRopaPrincipalSano, colorRopaInfectado, 0.45f) : colorRopaPrincipalSano);
        AplicarColor(rendersRopaSecundaria, infectado ? Mezclar(colorRopaSecundariaSano, colorRopaInfectado, 0.35f) : colorRopaSecundariaSano);
        AplicarColor(rendersCabello, colorCabelloSano);

        if (renderCuerpo != null)
            renderCuerpo.material.color = infectado ? Mezclar(colorRopaPrincipalSano, colorRopaInfectado, 0.45f) : colorRopaPrincipalSano;

        if (renderCabeza != null)
            renderCabeza.material.color = infectado ? colorPielInfectado : colorPielSano;
    }

    private void InicializarColoresBase()
    {
        colorRopaPrincipalSano = paletaRopaPrincipal[Random.Range(0, paletaRopaPrincipal.Length)];
        colorRopaSecundariaSano = paletaRopaSecundaria[Random.Range(0, paletaRopaSecundaria.Length)];
        colorCabelloSano = paletaCabello[Random.Range(0, paletaCabello.Length)];
    }

    private void AplicarColor(Renderer[] renders, Color color)
    {
        if (renders == null)
            return;

        foreach (var render in renders)
        {
            if (render != null)
                render.material.color = color;
        }
    }

    private Color Mezclar(Color colorBase, Color colorObjetivo, float factor)
    {
        return Color.Lerp(colorBase, colorObjetivo, Mathf.Clamp01(factor));
    }
}
