using UnityEngine;
using UnityEngine.AI;

// Automatiza una escena mínima si faltan elementos: administrador, metas y salida
public class EscenaBootstrap : MonoBehaviour
{
    public bool crearPuntosMeta = true;
    public int cantidadMetas = 3;

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

        // Punto de inicio si falta
        if (admin.puntoInicio == null)
        {
            var p = new GameObject("PuntoInicio");
            p.transform.position = Vector3.zero;
            admin.puntoInicio = p;
        }

        // NavMesh: si no hay superficie navegable, intenta crear un plano (no bokea NavMesh automáticamente)
        var anyNav = FindObjectOfType<NavMeshAgent>();
        if (anyNav == null)
        {
            var piso = GameObject.CreatePrimitive(PrimitiveType.Plane);
            piso.name = "Piso";
            piso.transform.position = Vector3.zero;
            piso.transform.localScale = new Vector3(5, 1, 5);
        }

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
                    m.transform.localScale = new Vector3(1, 0.2f, 1);
                    m.transform.position = new Vector3(Random.Range(-20f, 20f), 0.1f, Random.Range(-20f, 20f));
                    var col = m.GetComponent<Collider>();
                    if (col) { col.isTrigger = true; }
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
            s.transform.localScale = new Vector3(3, 0.2f, 1);
            s.transform.position = new Vector3(0, 0.1f, -25f);
            var col = s.GetComponent<Collider>();
            if (col) col.isTrigger = true;
        }
    }
}
