// Luchador.cs
using System.Collections;
using UnityEngine;

public class Luchador : MonoBehaviour
{
    public LuchadorData luchadorData; // El ScriptableObject que contiene los datos del luchador
    public ControladorTorneo controladorTorneo; // Referencia al controlador del torneo
    public float vidaActual;
    public Rigidbody rb;
    public Vector3 posicionAtaque; // Posici�n asignada para el ataque
    private Vector3 posicionInicial;
    private Vector3 escalaOriginal;
    private bool estaAtacando = false;
    private bool estaDeformando = false;

    void Start()
    {
        vidaActual = luchadorData.vidaMaxima;
        posicionInicial = transform.position;
        escalaOriginal = transform.localScale; // Almacenar la escala original

        // Asignar la imagen al material del cubo
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && luchadorData.imagen != null)
        {
            // Crear una instancia del material para evitar modificar el material original
            renderer.material = new Material(renderer.material);
            renderer.material.mainTexture = luchadorData.imagen;
        }
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

        // Calcular la direcci�n del ataque
        Vector3 direccionAtaque = (oponente.transform.position - transform.position).normalized;

        // Aplicar el ataque al oponente con la direcci�n
        oponente.RecibirGolpe(luchadorData.fuerza, direccionAtaque);

        // Revertir deformaci�n al regresar
        yield return StartCoroutine(DeformarCubo(false, 0.2f));

        // Regresar a la posici�n inicial
        yield return new WaitForSeconds(0.5f);
        transform.LookAt(posicionInicial); // Mirar hacia la posici�n inicial
        yield return StartCoroutine(MoverHacia(posicionInicial, tiempoMovimiento));

        estaAtacando = false;

        // Actualizar la barra de vida despu�s de atacar (si es necesario)
        controladorTorneo.ActualizarBarraVida(this);
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

    public void RecibirGolpe(float ataqueAtacante, Vector3 direccionAtaque)
    {
        // Determinar si el luchador esquiva el ataque
        float probabilidadEsquiva = 0.2f; // 20%
        if (Random.value <= probabilidadEsquiva)
        {
            // Esquiva el ataque
            Esquivar();
            controladorTorneo.MostrarEvento($"{luchadorData.nombre} ha esquivado el ataque!");
            return; // No aplicar da�o ni fuerza
        }

        // Calcular el da�o basado en el ataque y la resistencia
        float da�o = Mathf.Max(0, ataqueAtacante - luchadorData.resistencia);

        if (luchadorData.resistencia > ataqueAtacante)
        {
            da�o = ataqueAtacante * 0.1f; // Solo el 10% del ataque si la defensa es mayor
        }
        da�o = 100;
        // Aplicar el da�o a la salud
        vidaActual -= da�o;
        vidaActual = Mathf.Max(0, vidaActual); // Asegurarse de que la salud no sea negativa

        // Mostrar evento de da�o recibido
        controladorTorneo.MostrarEvento($"{luchadorData.nombre} recibe {da�o:F1} de da�o. Vida restante: {vidaActual:F1}");

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            controladorTorneo.MostrarEvento($"{luchadorData.nombre} ha sido derrotado!");
        }

        // Aplicar el empuje utilizando Rigidbody.AddForce
        Vector3 fuerzaPush = direccionAtaque * luchadorData.fuerza * 0.5f; // Ajusta el multiplicador seg�n sea necesario
        rb.AddForce(fuerzaPush, ForceMode.Impulse);

        // Actualizar la barra de vida despu�s de recibir da�o
        controladorTorneo.ActualizarBarraVida(this);
    }

    public bool EstaVivo()
    {
        return vidaActual > 0;
    }

    private IEnumerator DeformarCubo(bool estirando, float duracion)
    {
        if (estaDeformando) yield break; // Evitar deformaciones superpuestas

        estaDeformando = true;

        Vector3 escalaObjetivo;

        if (estirando)
        {
            escalaObjetivo = new Vector3(escalaOriginal.x, escalaOriginal.y, escalaOriginal.z * 1.5f);
        }
        else
        {
            escalaObjetivo = escalaOriginal;
        }

        Vector3 escalaInicialDeformada = transform.localScale;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            transform.localScale = Vector3.Lerp(escalaInicialDeformada, escalaObjetivo, tiempoTranscurrido / duracion);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        transform.localScale = escalaObjetivo;
        estaDeformando = false;
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

        Vector3 direccionEsquiva = Vector3.forward * -25f;
        Vector3 posicionObjetivo = transform.position + direccionEsquiva;
        float tiempoMovimiento = 0.3f;
        yield return StartCoroutine(MoverHacia(posicionObjetivo, tiempoMovimiento));

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoverHacia(posicionInicial, tiempoMovimiento));

        estaAtacando = false;
    }

    // M�todo para obtener el porcentaje de salud actual (0 a 1)
    public float GetHealthPercent()
    {
        return vidaActual / luchadorData.vidaMaxima;
    }


    // M�todo para resetear el estado del luchador entre combates
    public void ResetearEstado(LuchadorData nuevoLuchadorData)
    {
        luchadorData = nuevoLuchadorData;
        vidaActual = luchadorData.vidaMaxima; // Restablecer la vida
        transform.position = posicionInicial; // Volver a la posici�n inicial
        transform.localScale = escalaOriginal; // Restablecer la escala original
        GetComponent<Renderer>().material.mainTexture = luchadorData.imagen; // Actualizar la imagen del luchador, si es necesario
        rb.velocity = Vector3.zero; // Detener cualquier movimiento anterior
    }

}
