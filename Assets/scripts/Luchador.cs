using System.Collections;
using UnityEngine;

public class Luchador : MonoBehaviour
{
    public LuchadorData luchadorData; // El ScriptableObject que contiene los datos del luchador
    public ControladorTorneo controladorTorneo; // Referencia al controlador del torneo
    private float vidaActual;
    public Rigidbody rb;
    public Vector3 posicionAtaque; // Posici�n asignada para el ataque
    private Vector3 posicionInicial;
    private Vector3 escalaOriginal;
    private bool estaAtacando = false;

    void Start()
    {
        vidaActual = luchadorData.vidaMaxima;
        posicionInicial = transform.position;
        escalaOriginal = transform.localScale; // Almacenar la escala original
    }

    // Funci�n que inicia el ataque
    public IEnumerator Atacar(Luchador oponente)
    {
        if (!estaAtacando)
        {
            yield return StartCoroutine(RealizarAtaque(oponente));
        }
    }

    // Corrutina para manejar la animaci�n del ataque
    private IEnumerator RealizarAtaque(Luchador oponente)
    {
        estaAtacando = true;

        // Movimiento hacia la posici�n de ataque
        float tiempoMovimiento = 0.5f;
        yield return StartCoroutine(MoverHacia(posicionAtaque, tiempoMovimiento));

        // Mirar hacia el oponente
        transform.LookAt(oponente.transform.position);

        // Mostrar evento de ataque
        controladorTorneo.MostrarEvento($"{luchadorData.nombre} ataca a {oponente.luchadorData.nombre}");

        // Iniciar deformaci�n durante el ataque
        yield return StartCoroutine(DeformarCubo(true, 0.2f));

        // Aplicar la fuerza al oponente
        Vector3 direccionAtaque = (oponente.transform.position - transform.position).normalized;
        oponente.RecibirGolpe(direccionAtaque * luchadorData.fuerza);

        // Revertir deformaci�n al regresar
        yield return StartCoroutine(DeformarCubo(false, 0.2f));

        // Regresar a la posici�n inicial
        yield return new WaitForSeconds(0.5f);
        transform.LookAt(posicionInicial); // Mirar hacia la posici�n inicial
        yield return StartCoroutine(MoverHacia(posicionInicial, tiempoMovimiento));

        estaAtacando = false;
    }

    private IEnumerator MoverHacia(Vector3 objetivo, float tiempo)
    {
        Vector3 inicio = transform.position;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempo)
        {
            transform.position = Vector3.Lerp(inicio, objetivo, tiempoTranscurrido / tiempo);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        transform.position = objetivo;
    }

    public void RecibirGolpe(Vector3 fuerzaGolpe)
    {
        float escalaFuerza = 0.05f; 
        rb.AddForce(fuerzaGolpe * escalaFuerza, ForceMode.Impulse);

        float da�o = fuerzaGolpe.magnitude * escalaFuerza;
        vidaActual -= da�o;

        // Mostrar evento de da�o recibido
        controladorTorneo.MostrarEvento($"{luchadorData.nombre} recibe {da�o:F1} de da�o. Vida restante: {vidaActual:F1}");

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            controladorTorneo.MostrarEvento($"{luchadorData.nombre} ha sido derrotado!");
        }
    }

    public bool EstaVivo()
    {
        return vidaActual > 0;
    }

    private IEnumerator DeformarCubo(bool estirando, float duracion)
    {
        Vector3 escalaObjetivo;

        if (estirando)
        {
            escalaObjetivo = new Vector3(escalaOriginal.x, escalaOriginal.y, escalaOriginal.z * 1.5f);
        }
        else
        {
            escalaObjetivo = escalaOriginal;
        }

        Vector3 escalaInicial = transform.localScale;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            transform.localScale = Vector3.Lerp(escalaInicial, escalaObjetivo, tiempoTranscurrido / duracion);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        transform.localScale = escalaObjetivo;
    }

    // Opcional: M�todo para esquivar (sin cambios)
    public void Esquivar()
    {
        if (!estaAtacando)
        {
            StartCoroutine(RealizarEsquiva());
        }
    }

    private IEnumerator RealizarEsquiva()
    {
        estaAtacando = true;

        Vector3 direccionEsquiva = Vector3.right * 2f;
        Vector3 posicionObjetivo = transform.position + direccionEsquiva;
        float tiempoMovimiento = 0.3f;
        yield return StartCoroutine(MoverHacia(posicionObjetivo, tiempoMovimiento));

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoverHacia(posicionInicial, tiempoMovimiento));

        estaAtacando = false;
    }
}
