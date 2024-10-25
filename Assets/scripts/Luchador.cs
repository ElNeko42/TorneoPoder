using System.Collections;
using UnityEngine;

public class Luchador : MonoBehaviour
{
    public LuchadorData luchadorData; // El ScriptableObject que contiene los datos del luchador
    public ControladorTorneo controladorTorneo; // Referencia al controlador del torneo
    public float vidaActual;
    public Rigidbody rb;
    public Vector3 posicionAtaque; // Posición asignada para el ataque
    private Vector3 posicionInicial;
    private Vector3 escalaOriginal;
    private bool estaAtacando = false;
    private bool estaDeformando = false;
    public Transform puntoKi; // Punto donde se lanzará la bola de energía
    public GameObject ataqueEspecialPrefab; // Prefab de la bola de energía
    private AudioSource audioSourceEfectos;
    public AudioClip[] sonidoGolpes;
    public AudioClip sonidoAtaqueEspecial;
    public AudioClip sonidoEsquivar;
    public bool preparadoParaEsquivar = false;

    void Start()
    {
        // Inicializar variables de vida y posición
        vidaActual = luchadorData.vidaMaxima;
        posicionInicial = transform.position;
        escalaOriginal = transform.localScale;

        // Configurar punto de lanzamiento de Ki
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
            Debug.LogError("No se encontró ningún hijo con el tag 'ki'. Asegúrate de que el punto está correctamente asignado.");
        }

        // Configurar el material de imagen del luchador
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && luchadorData.imagen != null)
        {
            renderer.material = new Material(renderer.material);
            renderer.material.mainTexture = luchadorData.imagen;
        }

        // Buscar el GameObject con el tag "efectos" para asignar audioSourceEfectos
        GameObject efectosObj = GameObject.FindGameObjectWithTag("efectos");
        if (efectosObj != null)
        {
            audioSourceEfectos = efectosObj.GetComponent<AudioSource>();
            if (audioSourceEfectos == null)
            {
                Debug.LogError("No se encontró un AudioSource en el GameObject con el tag 'efectos'.");
            }
        }
        else
        {
            Debug.LogError("No se encontró ningún GameObject con el tag 'efectos'.");
        }
    }

    public IEnumerator Atacar(Luchador oponente)
    {
        if (!estaAtacando)
        {
            yield return StartCoroutine(RealizarAtaque(oponente));
        }
    }

    private IEnumerator RealizarAtaque(Luchador oponente)
    {
        estaAtacando = true;
        controladorTorneo.MostrarEvento($"{luchadorData.nombre} ataca a {oponente.luchadorData.nombre}");
        yield return StartCoroutine(DeformarCubo(true, 0.1f));

        if (Random.value <= 0.20f && this != controladorTorneo.luchadorInstancia1)
        {
            LanzarAtaqueEspecial(oponente);
        }
        else
        {
            float tiempoMovimiento = 0.3f;
            yield return StartCoroutine(MoverHacia(posicionAtaque, tiempoMovimiento));

            Quaternion rotacionOriginal = transform.rotation;
            Quaternion rotacionAtacando = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));
            transform.rotation = rotacionAtacando;
            transform.LookAt(oponente.transform.position);
            Vector3 direccionAtaque = (oponente.transform.position - transform.position).normalized;

            oponente.RecibirGolpe(luchadorData.fuerza, direccionAtaque, false);

            yield return StartCoroutine(DeformarCubo(false, 0.1f));
            transform.rotation = rotacionOriginal;
            yield return StartCoroutine(MoverHacia(posicionInicial, tiempoMovimiento));
        }

        estaAtacando = false;
        controladorTorneo.ActualizarBarraVida(this);
    }

    private void LanzarAtaqueEspecial(Luchador oponente)
    {
        GameObject ataqueEspecial = Instantiate(ataqueEspecialPrefab, puntoKi.position, Quaternion.identity);
        Vector3 direccion = (oponente.transform.position - puntoKi.position).normalized;
        ataqueEspecial.GetComponent<Rigidbody>().velocity = direccion * 50f;

        ataqueEspecial.AddComponent<BolaEnergia>().Inicializar(oponente, luchadorData.fuerza * 2);
        oponente.RecibirGolpe(luchadorData.fuerza * 2, direccion, true);
    }

    public IEnumerator AtacarEspecial(Luchador oponente)
    {
        if (!estaAtacando)
        {
            estaAtacando = true;
            controladorTorneo.MostrarEvento($"{luchadorData.nombre} lanza un ataque especial a {oponente.luchadorData.nombre}");
            if (audioSourceEfectos != null && sonidoAtaqueEspecial != null)
            {
                audioSourceEfectos.PlayOneShot(sonidoAtaqueEspecial);
            }
            LanzarAtaqueEspecial(oponente);
            yield return new WaitForSeconds(1f);
            estaAtacando = false;
            controladorTorneo.ActualizarBarraVida(this);
        }
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

    public void RecibirGolpe(float ataqueAtacante, Vector3 direccionAtaque, bool esAtaqueEspecial)
    {
        if (preparadoParaEsquivar)
        {
            Esquivar();
            controladorTorneo.MostrarEvento($"{luchadorData.nombre} ha esquivado el ataque!");
            preparadoParaEsquivar = false;
            return;
        }
        else if (!esAtaqueEspecial && Random.value <= 0.2f && this != controladorTorneo.luchadorInstancia1)
        {
            Esquivar();
            controladorTorneo.MostrarEvento($"{luchadorData.nombre} ha esquivado el ataque!");
            return;
        }

        // Calcular el daño
        float daño = Mathf.Max(0, ataqueAtacante - luchadorData.resistencia);
        daño = luchadorData.resistencia > ataqueAtacante ? ataqueAtacante * 0.1f : daño;
        vidaActual -= daño;
        vidaActual = Mathf.Max(0, vidaActual);

        // Reproducir sonido de golpe
        if (sonidoGolpes != null && sonidoGolpes.Length > 0 && audioSourceEfectos != null)
        {
            int indiceAleatorio = Random.Range(0, sonidoGolpes.Length);
            AudioClip sonidoSeleccionado = sonidoGolpes[indiceAleatorio];
            audioSourceEfectos.PlayOneShot(sonidoSeleccionado);
        }
        else
        {
            Debug.LogWarning("AudioSource o sonidoGolpes no asignado correctamente.");
        }

        // Mostrar el daño recibido
        controladorTorneo.MostrarEvento($"{luchadorData.nombre} recibe {daño:F1} de daño. Vida restante: {vidaActual:F1}");

        if (vidaActual <= 0)
        {
            controladorTorneo.MostrarEvento($"{luchadorData.nombre} ha sido derrotado!");
        }

        // Deformación exagerada al recibir golpe
        StartCoroutine(DeformarCubo(true, 0.1f));

        // Limitar la fuerza aplicada para que el luchador no sea empujado demasiado lejos
        Vector3 fuerzaExagerada = Vector3.ClampMagnitude(direccionAtaque * luchadorData.fuerza * 1.5f, 5f);

        if (rb != null)
        {
            rb.AddForce(fuerzaExagerada, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Rigidbody (rb) no asignado en el Luchador.");
        }

        // Revertir la deformación después de un tiempo corto
        StartCoroutine(DeformarCubo(false, 0.1f));

        // Actualizar barra de vida
        controladorTorneo.ActualizarBarraVida(this);
    }


    public bool EstaVivo()
    {
        return vidaActual > 0;
    }

    private IEnumerator DeformarCubo(bool estirando, float duracion)
    {
        if (estaDeformando) yield break;
        estaDeformando = true;

        Vector3 escalaObjetivo = estirando
            ? new Vector3(escalaOriginal.x * 1.2f, escalaOriginal.y * 0.8f, escalaOriginal.z * 1.5f)
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
        Vector3 direccionEsquiva = Vector3.back * 2f;
        Vector3 posicionObjetivo = transform.position + direccionEsquiva;
        float tiempoMovimiento = 0.3f;
        yield return StartCoroutine(MoverHacia(posicionObjetivo, tiempoMovimiento));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoverHacia(posicionInicial, tiempoMovimiento));

        if (audioSourceEfectos != null && sonidoEsquivar != null)
        {
            audioSourceEfectos.PlayOneShot(sonidoEsquivar);
        }

        estaAtacando = false;
    }

    public void PrepararEsquivar()
    {
        preparadoParaEsquivar = true;
    }

    public float GetHealthPercent()
    {
        return vidaActual / luchadorData.vidaMaxima;
    }

    public void ResetearEstado(LuchadorData nuevoLuchadorData)
    {
        luchadorData = nuevoLuchadorData;
        vidaActual = luchadorData.vidaMaxima;
        transform.position = posicionInicial;
        transform.localScale = escalaOriginal;
        GetComponent<Renderer>().material.mainTexture = luchadorData.imagen;
        if (rb != null) rb.velocity = Vector3.zero;
    }
}
