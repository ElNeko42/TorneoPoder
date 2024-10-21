using System.Collections;
using UnityEngine;

public class Luchador : MonoBehaviour
{
    public string nombre;
    [Range(0, 100)] public int fuerza;
    [Range(0, 100)] public int velocidad;
    [Range(0, 100)] public int resistencia;
    public float vida = 100f;
    public Rigidbody rb;

    // Posici�n inicial para regresar despu�s del ataque/esquiva
    private Vector3 posicionInicial;
    private bool estaAtacando = false;

    void Start()
    {
        // Guardamos la posici�n inicial del luchador
        posicionInicial = transform.position;
    }

    // Funci�n que inicia el ataque
    public void Atacar(Luchador oponente)
    {
        if (!estaAtacando)
        {
            StartCoroutine(RealizarAtaque(oponente));
        }
    }

    // Corrutina para manejar la animaci�n del ataque
    private IEnumerator RealizarAtaque(Luchador oponente)
    {
        estaAtacando = true;

        // Movimiento hacia el oponente
        Vector3 direccionAtaque = (oponente.transform.position - transform.position).normalized;
        Vector3 posicionObjetivo = oponente.transform.position - direccionAtaque * 1.5f; // Un poco antes del contacto
        float tiempoMovimiento = 0.5f;
        yield return StartCoroutine(MoverHacia(posicionObjetivo, tiempoMovimiento));

        // Aplicar la fuerza al oponente
        oponente.RecibirGolpe(direccionAtaque * fuerza);

        // Regresar a la posici�n inicial
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoverHacia(posicionInicial, tiempoMovimiento));

        estaAtacando = false;
    }

    // Funci�n para mover al luchador hacia una posici�n
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

    // Funci�n que recibe el golpe y aplica una fuerza al luchador
    public void RecibirGolpe(Vector3 fuerzaGolpe)
    {
        rb.AddForce(fuerzaGolpe, ForceMode.Impulse);
    }

    // Funci�n para esquivar el ataque
    public void Esquivar()
    {
        if (!estaAtacando)
        {
            StartCoroutine(RealizarEsquiva());
        }
    }

    // Corrutina para la esquiva
    private IEnumerator RealizarEsquiva()
    {
        estaAtacando = true;

        // Moverse hacia un lado para esquivar
        Vector3 direccionEsquiva = Vector3.right * 2f; // Puedes cambiar la direcci�n de la esquiva
        Vector3 posicionObjetivo = transform.position + direccionEsquiva;
        float tiempoMovimiento = 0.3f;
        yield return StartCoroutine(MoverHacia(posicionObjetivo, tiempoMovimiento));

        // Volver a la posici�n original despu�s de esquivar
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(MoverHacia(posicionInicial, tiempoMovimiento));

        estaAtacando = false;
    }
}
