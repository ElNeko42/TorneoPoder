using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objetivoActual; // Luchador que ser� el objetivo de la c�mara
    public float velocidadMovimiento = 5f; // Velocidad con la que la c�mara sigue al objetivo
    public Vector3 offset; // Desplazamiento opcional para ajustar la posici�n de la c�mara

    private void Update()
    {
        if (objetivoActual != null)
        {
            // Posici�n deseada de la c�mara, ajustada por el offset
            Vector3 posicionDeseada = new Vector3(objetivoActual.position.x, objetivoActual.position.y, transform.position.z) + offset;

            // Mover la c�mara suavemente hacia el objetivo en los ejes X e Y
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadMovimiento * Time.deltaTime);
        }
    }

    // M�todo para actualizar el objetivo de la c�mara
    public void EstablecerObjetivo(Transform nuevoObjetivo)
    {
        objetivoActual = nuevoObjetivo;
    }

    // M�todo para el temblor de la c�mara
    public IEnumerator TemblorCamara(float duracion, float magnitud)
    {
        Vector3 posicionOriginal = transform.localPosition;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            // Generar un peque�o desplazamiento en X e Y para el temblor
            float desplazamientoX = Random.Range(-1f, 1f) * magnitud;
            float desplazamientoY = Random.Range(-1f, 1f) * magnitud;

            transform.localPosition = new Vector3(posicionOriginal.x + desplazamientoX, posicionOriginal.y + desplazamientoY, posicionOriginal.z);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Volver a la posici�n original despu�s del temblor
        transform.localPosition = posicionOriginal;
    }
}
    