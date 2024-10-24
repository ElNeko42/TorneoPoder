using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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
    public Transform puntoKi; // Punto donde se lanzar� la bola de energ�a
    public GameObject ataqueEspecialPrefab; // Prefab de la bola de energ�a

    void Start()
    {
        vidaActual = luchadorData.vidaMaxima;
        posicionInicial = transform.position;
        escalaOriginal = transform.localScale; // Almacenar la escala original

        // Buscar el transform del hijo con el tag "ki"
        foreach (Transform child in transform)
        {
            if (child.CompareTag("ki"))
            {
                puntoKi = child;
                break;
            }
        }

        if (puntoKi == null)
        {
            Debug.LogError("No se encontr� ning�n hijo con el tag 'ki'. Aseg�rate de que el punto est� correctamente asignado.");
        }

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
        // Establecer el enfoque de la c�mara en este luchador antes de atacar
        Camera.main.GetComponent<CameraController>().EstablecerObjetivo(transform);

        // Movimiento hacia la posici�n de ataque
        float tiempoMovimiento = 0.3f;
        yield return StartCoroutine(MoverHacia(posicionAtaque, tiempoMovimiento));

        // Agregar una peque�a rotaci�n r�pida al final del ataque
        Quaternion rotacionOriginal = transform.rotation;
        Quaternion rotacionAtacando = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));
        transform.rotation = rotacionAtacando;

        // Mirar hacia el oponente
        transform.LookAt(oponente.transform.position);

        // Mostrar evento de ataque
        controladorTorneo.MostrarEvento($"{luchadorData.nombre} ataca a {oponente.luchadorData.nombre}");

        // Deformaci�n exagerada durante el ataque
        yield return StartCoroutine(DeformarCubo(true, 0.1f));

        // Probabilidad de lanzar ataque especial (20%)
        if (Random.value <= 0.10f)
        {
            LanzarAtaqueEspecial(oponente); // Lanza la bola de energ�a
        }
        else
        {
            // Calcular la direcci�n del ataque
            Vector3 direccionAtaque = (oponente.transform.position - transform.position).normalized;

            // Aplicar el ataque al oponente con la direcci�n
            oponente.RecibirGolpe(luchadorData.fuerza, direccionAtaque);
        }

        // Revertir la deformaci�n y rotaci�n al regresar
        yield return StartCoroutine(DeformarCubo(false, 0.1f));
        transform.rotation = rotacionOriginal;

        // Regresar a la posici�n inicial
        yield return new WaitForSeconds(0.2f); // Tiempo de espera m�s corto
        yield return StartCoroutine(MoverHacia(posicionInicial, tiempoMovimiento));

        estaAtacando = false;

        // Actualizar la barra de vida despu�s de atacar
        controladorTorneo.ActualizarBarraVida(this);

    }

    // Funci�n para lanzar la bola de energ�a
    private void LanzarAtaqueEspecial(Luchador oponente)
    {
        GameObject ataqueEspecial = Instantiate(ataqueEspecialPrefab, puntoKi.position, Quaternion.identity);
        Vector3 direccion = (oponente.transform.position - puntoKi.position).normalized;
        ataqueEspecial.GetComponent<Rigidbody>().velocity = direccion * 10f;

        // A�adir l�gica para destruir la bola y aplicar el da�o especial
        ataqueEspecial.AddComponent<BolaEnergia>().Inicializar(oponente, luchadorData.fuerza * 2); // Da�o triplicado
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
        // Esquivar
        if (Random.value <= 0.2f)
        {
            Esquivar();
            controladorTorneo.MostrarEvento($"{luchadorData.nombre} ha esquivado el ataque!");
            return;
        }

        // Calcular el da�o
        float da�o = Mathf.Max(0, ataqueAtacante - luchadorData.resistencia);
        da�o = luchadorData.resistencia > ataqueAtacante ? ataqueAtacante * 0.1f : da�o;
        vidaActual -= da�o;
        vidaActual = Mathf.Max(0, vidaActual);

        // Mostrar el da�o recibido
        controladorTorneo.MostrarEvento($"{luchadorData.nombre} recibe {da�o:F1} de da�o. Vida restante: {vidaActual:F1}");

        if (vidaActual <= 0)
        {
            controladorTorneo.MostrarEvento($"{luchadorData.nombre} ha sido derrotado!");
        }

        // Deformaci�n exagerada al recibir golpe
        StartCoroutine(DeformarCubo(true, 0.1f));

        // Limitar la fuerza aplicada para que el luchador no sea empujado demasiado lejos
        Vector3 fuerzaExagerada = direccionAtaque * luchadorData.fuerza * 1.5f; // Aplicar fuerza
        fuerzaExagerada = Vector3.ClampMagnitude(fuerzaExagerada, 5f); // Limitar la magnitud de la fuerza a 5 unidades (puedes ajustar este valor)

        rb.AddForce(fuerzaExagerada, ForceMode.Impulse);

        // Revertir la deformaci�n despu�s de un tiempo corto
        StartCoroutine(DeformarCubo(false, 0.1f));

        // Actualizar barra de vida
        controladorTorneo.ActualizarBarraVida(this);
        Camera.main.GetComponent<CameraController>().StartCoroutine(Camera.main.GetComponent<CameraController>().TemblorCamara(0.2f, 0.3f));
    }


    public bool EstaVivo()
    {
        return vidaActual > 0;
    }

    private IEnumerator DeformarCubo(bool estirando, float duracion)
    {
        if (estaDeformando) yield break; // Evitar deformaciones superpuestas
        estaDeformando = true;

        Vector3 escalaObjetivo = estirando
            ? new Vector3(escalaOriginal.x * 1.2f, escalaOriginal.y * 0.8f, escalaOriginal.z * 1.5f)  // Exagerar estiramiento
            : escalaOriginal;

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

    public float GetHealthPercent()
    {
        return vidaActual / luchadorData.vidaMaxima;
    }

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