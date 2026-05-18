using UnityEngine;
using UnityEngine.AI;

// Automatiza una escena mínima si faltan elementos: administrador, metas y salida
public class EscenaBootstrap : MonoBehaviour
{
    public bool crearPuntosMeta = true;
    public int cantidadMetas = 3;
    public bool crearMurosPerimetrales = true;
    public bool crearLocalesDecorativos = true;
    public bool crearDetallesPasillo = true;
    public float semianchoEscenario = 22f;
    public float semilargoEscenario = 22f;

    void Start()
    {
        // Administrador
        var admin = FindObjectOfType<Administrador>();
        if (admin == null)
        {
            var go = new GameObject("Administrador");
            admin = go.AddComponent<Administrador>();
            go.transform.position = Vector3.zero;
        }

        GameObject piso = ObtenerOCrearPiso();

        // Punto de inicio si falta
        if (admin.puntoInicio == null)
        {
            var p = new GameObject("PuntoInicio");
            p.transform.position = new Vector3(-semianchoEscenario + 4f, 0.2f, 0f);
            admin.puntoInicio = p;
        }

        if (crearMurosPerimetrales)
            CrearMurosSiFaltan();

        if (crearLocalesDecorativos)
            CrearLocalesSiFaltan();

        if (crearDetallesPasillo)
            CrearDetallesPasilloSiFaltan();

        // Crear metas
        if (crearPuntosMeta)
        {
            var metas = GameObject.FindGameObjectsWithTag("meta");
            if (metas.Length == 0)
            {
                for (int i = 0; i < cantidadMetas; i++)
                {
                    var m = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    m.name = $"Meta_{i+1}";
                    m.tag = "meta";
                    m.transform.localScale = new Vector3(1.2f, 0.2f, 1.2f);
                    m.transform.position = new Vector3(Random.Range(-semianchoEscenario + 4f, semianchoEscenario - 4f), 0.1f, Random.Range(-semilargoEscenario + 4f, semilargoEscenario - 4f));
                    m.GetComponent<Renderer>().material.color = new Color(0.22f, 0.75f, 0.45f);
                    var col = m.GetComponent<Collider>();
                    if (col) 
                    { 
                        col.isTrigger = true; 
                    }
                }
            }
        }

        // Crear salida
        var salida = GameObject.FindGameObjectWithTag("salida_cc");
        if (salida == null)
        {
            var s = GameObject.CreatePrimitive(PrimitiveType.Cube);
            s.name = "Salida_CC";
            s.tag = "salida_cc";
            s.transform.localScale = new Vector3(4f, 2.2f, 1f);
            s.transform.position = new Vector3(0f, 1.1f, -semilargoEscenario);
            s.GetComponent<Renderer>().material.color = new Color(0.93f, 0.57f, 0.15f);
            var col = s.GetComponent<Collider>();
            if (col) 
            { 
                col.isTrigger = true; 
            }
        }

        AlinearObjetosClaveConNavMesh(admin);

        ConfigurarIluminacion();
        ConfigurarCamara(piso.transform.position);
    }

    private void AlinearObjetosClaveConNavMesh(Administrador admin)
    {
        if (!HayNavMeshCercano(Vector3.zero, Mathf.Max(semianchoEscenario, semilargoEscenario) + 8f))
            return;

        if (admin != null && admin.puntoInicio != null)
            AjustarTransformAlNavMesh(admin.puntoInicio.transform, 4f, false);

        foreach (var meta in GameObject.FindGameObjectsWithTag("meta"))
            AjustarTransformAlNavMesh(meta.transform, 6f, true);

        var salida = GameObject.FindGameObjectWithTag("salida_cc");
        if (salida != null)
            AjustarTransformAlNavMesh(salida.transform, 6f, true);
    }

    private bool HayNavMeshCercano(Vector3 centro, float radio)
    {
        return NavMesh.SamplePosition(centro, out _, radio, NavMesh.AllAreas);
    }

    private void AjustarTransformAlNavMesh(Transform objetivo, float radioBusqueda, bool conservarAltura)
    {
        if (objetivo == null)
            return;

        if (!NavMesh.SamplePosition(objetivo.position, out NavMeshHit hit, radioBusqueda, NavMesh.AllAreas))
            return;

        objetivo.position = conservarAltura
            ? new Vector3(hit.position.x, objetivo.position.y, hit.position.z)
            : hit.position;
    }

    private GameObject ObtenerOCrearPiso()
    {
        var pisoExistente = GameObject.Find("Piso");
        if (pisoExistente != null)
            return pisoExistente;

        var piso = GameObject.CreatePrimitive(PrimitiveType.Plane);
        piso.name = "Piso";
        piso.transform.position = Vector3.zero;
        piso.transform.localScale = new Vector3(semianchoEscenario / 5f, 1f, semilargoEscenario / 5f);
        piso.GetComponent<Renderer>().material.color = new Color(0.83f, 0.84f, 0.82f);
        return piso;
    }

    private void CrearMurosSiFaltan()
    {
        if (GameObject.FindGameObjectsWithTag("muros").Length > 0)
            return;

        CrearMuro("Muro_Norte", new Vector3(0f, 2f, semilargoEscenario), new Vector3(semianchoEscenario * 2f, 4f, 1f));
        CrearMuro("Muro_Sur", new Vector3(0f, 2f, -semilargoEscenario), new Vector3(semianchoEscenario * 2f, 4f, 1f));
        CrearMuro("Muro_Este", new Vector3(semianchoEscenario, 2f, 0f), new Vector3(1f, 4f, semilargoEscenario * 2f));
        CrearMuro("Muro_Oeste", new Vector3(-semianchoEscenario, 2f, 0f), new Vector3(1f, 4f, semilargoEscenario * 2f));
    }

    private void CrearMuro(string nombre, Vector3 posicion, Vector3 escala)
    {
        var muro = GameObject.CreatePrimitive(PrimitiveType.Cube);
        muro.name = nombre;
        muro.tag = "muros";
        muro.transform.position = posicion;
        muro.transform.localScale = escala;
        muro.GetComponent<Renderer>().material.color = new Color(0.84f, 0.86f, 0.9f);
    }

    private void CrearLocalesSiFaltan()
    {
        if (GameObject.Find("LocalesDecorativos") != null)
            return;

        var contenedor = new GameObject("LocalesDecorativos");
        CrearHileraLocales(contenedor.transform, "LocalesNorte", semilargoEscenario - 1.4f, false);
        CrearHileraLocales(contenedor.transform, "LocalesSur", -semilargoEscenario + 1.4f, true);
        CrearIslasDecorativas(contenedor.transform);
        CrearEntradaPrincipal(contenedor.transform);
    }

    private void CrearHileraLocales(Transform padre, string nombre, float posicionZ, bool voltear)
    {
        var grupo = new GameObject(nombre);
        grupo.transform.SetParent(padre, false);

        int cantidadLocales = 6;
        float anchoLocal = 5.6f;
        float inicioX = -((cantidadLocales - 1) * anchoLocal) * 0.5f;

        for (int i = 0; i < cantidadLocales; i++)
        {
            float x = inicioX + i * anchoLocal;
            CrearLocal(grupo.transform, i, new Vector3(x, 0f, posicionZ), voltear);
        }
    }

    private void CrearLocal(Transform padre, int indice, Vector3 posicionBase, bool voltear)
    {
        var local = new GameObject($"Local_{indice + 1}");
        local.transform.SetParent(padre, false);
        local.transform.position = posicionBase;
        local.transform.rotation = Quaternion.Euler(0f, voltear ? 180f : 0f, 0f);

        CrearBloqueDecorativo(local.transform, "Fachada", new Vector3(0f, 1.8f, 0f), new Vector3(4.8f, 3.4f, 0.35f), new Color(0.93f, 0.94f, 0.96f));
        CrearBloqueDecorativo(local.transform, "MarcoSuperior", new Vector3(0f, 3.2f, -0.18f), new Vector3(4.9f, 0.32f, 0.3f), Color.Lerp(new Color(0.16f, 0.2f, 0.28f), Color.white, 0.1f));
        CrearBloqueDecorativo(local.transform, "VitrinaIzquierda", new Vector3(-1.35f, 1.55f, -0.2f), new Vector3(1.55f, 2.2f, 0.12f), new Color(0.63f, 0.83f, 0.92f));
        CrearBloqueDecorativo(local.transform, "VitrinaDerecha", new Vector3(1.35f, 1.55f, -0.2f), new Vector3(1.55f, 2.2f, 0.12f), new Color(0.63f, 0.83f, 0.92f));
        CrearBloqueDecorativo(local.transform, "Puerta", new Vector3(0f, 1.25f, -0.19f), new Vector3(0.95f, 1.95f, 0.1f), new Color(0.34f, 0.37f, 0.44f));
        CrearBloqueDecorativo(local.transform, "Aviso", new Vector3(0f, 2.95f, -0.28f), new Vector3(2.4f, 0.38f, 0.1f), ObtenerColorAviso(indice));
    }

    private void CrearIslasDecorativas(Transform padre)
    {
        for (int i = 0; i < 3; i++)
        {
            var isla = new GameObject($"Isla_{i + 1}");
            isla.transform.SetParent(padre, false);
            isla.transform.position = new Vector3(-10f + (i * 10f), 0f, 0f);
            CrearBloqueDecorativo(isla.transform, "Base", new Vector3(0f, 0.45f, 0f), new Vector3(2.8f, 0.9f, 1.8f), new Color(0.84f, 0.8f, 0.73f));
            CrearBloqueDecorativo(isla.transform, "Cubierta", new Vector3(0f, 1.4f, 0f), new Vector3(2.2f, 0.16f, 1.2f), new Color(0.7f, 0.22f, 0.2f));
        }
    }

    private void CrearEntradaPrincipal(Transform padre)
    {
        var entrada = new GameObject("EntradaPrincipal");
        entrada.transform.SetParent(padre, false);
        entrada.transform.position = new Vector3(-semianchoEscenario + 1.4f, 0f, 0f);

        CrearBloqueDecorativo(entrada.transform, "ArcoSuperior", new Vector3(0f, 3.1f, 0f), new Vector3(0.8f, 0.35f, 8.4f), new Color(0.2f, 0.24f, 0.3f));
        CrearBloqueDecorativo(entrada.transform, "Columna1", new Vector3(0f, 1.4f, 3.6f), new Vector3(0.7f, 2.8f, 0.7f), new Color(0.85f, 0.87f, 0.89f));
        CrearBloqueDecorativo(entrada.transform, "Columna2", new Vector3(0f, 1.4f, -3.6f), new Vector3(0.7f, 2.8f, 0.7f), new Color(0.85f, 0.87f, 0.89f));
        CrearBloqueDecorativo(entrada.transform, "AvisoEntrada", new Vector3(0.1f, 3.14f, 0f), new Vector3(0.16f, 0.2f, 3.4f), new Color(0.86f, 0.34f, 0.22f));
    }

    private void CrearDetallesPasilloSiFaltan()
    {
        if (GameObject.Find("DetallesPasillo") != null)
            return;

        var detalles = new GameObject("DetallesPasillo");
        CrearBloqueDecorativo(detalles.transform, "FranjaCentral", new Vector3(0f, 0.02f, 0f), new Vector3(5.8f, 0.02f, semilargoEscenario * 1.5f), new Color(0.73f, 0.76f, 0.8f));
        CrearBloqueDecorativo(detalles.transform, "BordeCentralIzq", new Vector3(-3.1f, 0.03f, 0f), new Vector3(0.12f, 0.03f, semilargoEscenario * 1.4f), new Color(0.96f, 0.96f, 0.96f));
        CrearBloqueDecorativo(detalles.transform, "BordeCentralDer", new Vector3(3.1f, 0.03f, 0f), new Vector3(0.12f, 0.03f, semilargoEscenario * 1.4f), new Color(0.96f, 0.96f, 0.96f));

        for (int i = 0; i < 5; i++)
        {
            float z = -16f + i * 8f;
            CrearBloqueDecorativo(detalles.transform, $"LuzPasillo_{i}", new Vector3(0f, 4.5f, z), new Vector3(6.2f, 0.08f, 0.45f), new Color(1f, 0.96f, 0.82f));
        }
    }

    private void CrearBloqueDecorativo(Transform padre, string nombre, Vector3 posicionLocal, Vector3 escalaLocal, Color color)
    {
        var pieza = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pieza.name = nombre;
        pieza.transform.SetParent(padre, false);
        pieza.transform.localPosition = posicionLocal;
        pieza.transform.localScale = escalaLocal;
        var render = pieza.GetComponent<Renderer>();
        if (render != null)
            render.material.color = color;

        var collider = pieza.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);
    }

    private Color ObtenerColorAviso(int indice)
    {
        Color[] colores =
        {
            new Color(0.86f, 0.33f, 0.25f),
            new Color(0.17f, 0.58f, 0.42f),
            new Color(0.2f, 0.45f, 0.78f),
            new Color(0.74f, 0.57f, 0.18f)
        };

        return colores[indice % colores.Length];
    }

    private void ConfigurarIluminacion()
    {
        if (FindObjectOfType<Light>() != null)
            return;

        var luz = new GameObject("LuzDireccional");
        var componenteLuz = luz.AddComponent<Light>();
        componenteLuz.type = LightType.Directional;
        componenteLuz.intensity = 1.1f;
        luz.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
    }

    private void ConfigurarCamara(Vector3 focoEscena)
    {
        Camera camara = Camera.main;
        if (camara == null)
        {
            var camaraGO = new GameObject("Main Camera");
            camaraGO.tag = "MainCamera";
            camara = camaraGO.AddComponent<Camera>();
            camaraGO.AddComponent<AudioListener>();
        }

        camara.clearFlags = CameraClearFlags.Skybox;
        camara.fieldOfView = 58f;
        camara.transform.position = focoEscena + new Vector3(0f, 26f, -28f);
        camara.transform.rotation = Quaternion.Euler(34f, 0f, 0f);

        var control = camara.GetComponent<ControlCamaraSimulacion>();
        if (control == null)
            control = camara.gameObject.AddComponent<ControlCamaraSimulacion>();

        control.EstablecerFoco(focoEscena);
    }
}
