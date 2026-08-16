using UnityEngine;
using UnityEngine.EventSystems;

public class ControlCamaraSimulacion : MonoBehaviour
{
    public float velocidadOrbitacion = 180f;
    public float velocidadPaneo = 0.4f;
    public float velocidadZoom = 10f;
    public float distanciaMinima = 10f;
    public float distanciaMaxima = 60f;
    public float inclinacionMinima = 20f;
    public float inclinacionMaxima = 75f;

    private Vector3 focoEscena = Vector3.zero;
    private float yaw = 0f;
    private float pitch = 40f;
    private float distanciaActual = 30f;

    private void Start()
    {
        var angulos = transform.eulerAngles;
        yaw = angulos.y;
        pitch = angulos.x;
        distanciaActual = Vector3.Distance(transform.position, focoEscena);
        if (distanciaActual <= 0.01f)
            distanciaActual = 30f;

        ActualizarTransformacion();
    }

    private void LateUpdate()
    {
        bool punteroSobreUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if (!punteroSobreUI && Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * velocidadOrbitacion * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * velocidadOrbitacion * 0.6f * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, inclinacionMinima, inclinacionMaxima);
        }

        if (!punteroSobreUI && Input.GetMouseButton(2))
        {
            Vector3 derecha = transform.right;
            Vector3 adelantePlano = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 delta = (-Input.GetAxis("Mouse X") * derecha + -Input.GetAxis("Mouse Y") * adelantePlano) * velocidadPaneo * distanciaActual * Time.deltaTime;
            focoEscena += delta;
        }

        if (!punteroSobreUI)
        {
            distanciaActual -= Input.mouseScrollDelta.y * velocidadZoom;
            distanciaActual = Mathf.Clamp(distanciaActual, distanciaMinima, distanciaMaxima);
        }

        Vector3 paneoTeclado = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (paneoTeclado.sqrMagnitude > 0.01f)
        {
            Vector3 derecha = transform.right;
            Vector3 adelantePlano = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            focoEscena += (derecha * paneoTeclado.x + adelantePlano * paneoTeclado.z) * (velocidadPaneo * 12f) * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            yaw = 0f;
            pitch = 40f;
            distanciaActual = 30f;
        }

        ActualizarTransformacion();
    }

    public void EstablecerFoco(Vector3 nuevoFoco)
    {
        focoEscena = nuevoFoco;
        if (Application.isPlaying)
            ActualizarTransformacion();
    }

    private void ActualizarTransformacion()
    {
        Quaternion rotacion = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 direccion = rotacion * Vector3.forward;
        transform.position = focoEscena - direccion * distanciaActual;
        transform.rotation = rotacion;
    }
}
