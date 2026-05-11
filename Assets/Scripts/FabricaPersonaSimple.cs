using System.Collections.Generic;
using UnityEngine;

public static class FabricaPersonaSimple
{
    public static AspectoAgente ConstruirPersona(GameObject raiz)
    {
        var contenedorVisual = new GameObject("VisualPersona");
        contenedorVisual.transform.SetParent(raiz.transform, false);

        var rendersPiel = new List<Renderer>();
        var rendersRopaPrincipal = new List<Renderer>();
        var rendersRopaSecundaria = new List<Renderer>();
        var rendersCabello = new List<Renderer>();

        var torso = CrearParte(PrimitiveType.Cube, "Torso", contenedorVisual.transform, new Vector3(0f, 1.25f, 0f), new Vector3(0.48f, 0.72f, 0.26f), rendersRopaPrincipal);
        var cadera = CrearParte(PrimitiveType.Cube, "Cadera", contenedorVisual.transform, new Vector3(0f, 0.82f, 0f), new Vector3(0.42f, 0.2f, 0.22f), rendersRopaSecundaria);
        var cabeza = CrearParte(PrimitiveType.Sphere, "Cabeza", contenedorVisual.transform, new Vector3(0f, 1.88f, 0f), new Vector3(0.32f, 0.36f, 0.32f), rendersPiel);
        var cabello = CrearParte(PrimitiveType.Sphere, "Cabello", contenedorVisual.transform, new Vector3(0f, 2.02f, -0.02f), new Vector3(0.34f, 0.2f, 0.34f), rendersCabello);
        cabello.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        var brazoIzquierdo = CrearParte(PrimitiveType.Cylinder, "BrazoIzquierdo", contenedorVisual.transform, new Vector3(-0.34f, 1.22f, 0f), new Vector3(0.08f, 0.35f, 0.08f), rendersRopaPrincipal, new Vector3(0f, 0f, 16f));
        var brazoDerecho = CrearParte(PrimitiveType.Cylinder, "BrazoDerecho", contenedorVisual.transform, new Vector3(0.34f, 1.22f, 0f), new Vector3(0.08f, 0.35f, 0.08f), rendersRopaPrincipal, new Vector3(0f, 0f, -16f));
        var antebrazoIzquierdo = CrearParte(PrimitiveType.Cylinder, "AntebrazoIzquierdo", contenedorVisual.transform, new Vector3(-0.41f, 0.88f, 0f), new Vector3(0.07f, 0.28f, 0.07f), rendersPiel, new Vector3(0f, 0f, 12f));
        var antebrazoDerecho = CrearParte(PrimitiveType.Cylinder, "AntebrazoDerecho", contenedorVisual.transform, new Vector3(0.41f, 0.88f, 0f), new Vector3(0.07f, 0.28f, 0.07f), rendersPiel, new Vector3(0f, 0f, -12f));

        var piernaIzquierda = CrearParte(PrimitiveType.Cylinder, "PiernaIzquierda", contenedorVisual.transform, new Vector3(-0.14f, 0.38f, 0f), new Vector3(0.1f, 0.42f, 0.1f), rendersRopaSecundaria);
        var piernaDerecha = CrearParte(PrimitiveType.Cylinder, "PiernaDerecha", contenedorVisual.transform, new Vector3(0.14f, 0.38f, 0f), new Vector3(0.1f, 0.42f, 0.1f), rendersRopaSecundaria);
        CrearParte(PrimitiveType.Cube, "ZapatoIzquierdo", contenedorVisual.transform, new Vector3(-0.14f, 0.02f, 0.06f), new Vector3(0.14f, 0.05f, 0.24f), rendersCabello);
        CrearParte(PrimitiveType.Cube, "ZapatoDerecho", contenedorVisual.transform, new Vector3(0.14f, 0.02f, 0.06f), new Vector3(0.14f, 0.05f, 0.24f), rendersCabello);

        var aspecto = raiz.AddComponent<AspectoAgente>();
        aspecto.renderCuerpo = torso.GetComponent<Renderer>();
        aspecto.renderCabeza = cabeza.GetComponent<Renderer>();
        aspecto.rendersPiel = rendersPiel.ToArray();
        aspecto.rendersRopaPrincipal = rendersRopaPrincipal.ToArray();
        aspecto.rendersRopaSecundaria = rendersRopaSecundaria.ToArray();
        aspecto.rendersCabello = rendersCabello.ToArray();

        var animacion = raiz.AddComponent<AnimacionPersonaSimple>();
        animacion.visualRaiz = contenedorVisual.transform;
        animacion.brazoIzquierdo = brazoIzquierdo.transform;
        animacion.brazoDerecho = brazoDerecho.transform;
        animacion.antebrazoIzquierdo = antebrazoIzquierdo.transform;
        animacion.antebrazoDerecho = antebrazoDerecho.transform;
        animacion.piernaIzquierda = piernaIzquierda.transform;
        animacion.piernaDerecha = piernaDerecha.transform;

        return aspecto;
    }

    private static GameObject CrearParte(PrimitiveType tipo, string nombre, Transform padre, Vector3 posicionLocal, Vector3 escalaLocal, List<Renderer> listaDestino, Vector3? rotacionLocal = null)
    {
        var parte = GameObject.CreatePrimitive(tipo);
        parte.name = nombre;
        parte.transform.SetParent(padre, false);
        parte.transform.localPosition = posicionLocal;
        parte.transform.localScale = escalaLocal;
        if (rotacionLocal.HasValue)
            parte.transform.localRotation = Quaternion.Euler(rotacionLocal.Value);

        var collider = parte.GetComponent<Collider>();
        if (collider != null)
            Object.Destroy(collider);

        var render = parte.GetComponent<Renderer>();
        if (render != null)
            listaDestino.Add(render);

        return parte;
    }
}
