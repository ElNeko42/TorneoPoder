using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objetivoActual; // Luchador que será el objetivo de la cámara
    public float velocidadMovimiento = 5f; // Velocidad con la que la cámara sigue al objetivo
    public Vector3 offset; // Desplazamiento opcional para ajustar la posición de la cámara

    private void Update()
    {
        if (objetivoActual != null)
        {
            // Posición deseada de la cámara, ajustada por el offset
            Vector3 posicionDeseada = new Vector3(objetivoActual.position.x, objetivoActual.position.y, transform.position.z) + offset;

            // Mover la cámara suavemente hacia el objetivo en los ejes X e Y
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, velocidadMovimiento * Time.deltaTime);
        }
    }

    // Método para actualizar el objetivo de la cámara
    public void EstablecerObjetivo(Transform nuevoObjetivo)
    {
        objetivoActual = nuevoObjetivo;
    }

    // Método para el temblor de la cámara
    public IEnumerator TemblorCamara(float duracion, float magnitud)
    {
        Vector3 posicionOriginal = transform.localPosition;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            // Generar un pequeño desplazamiento en X e Y para el temblor
            float desplazamientoX = Random.Range(-1f, 1f) * magnitud;
            float desplazamientoY = Random.Range(-1f, 1f) * magnitud;

            transform.localPosition = new Vector3(posicionOriginal.x + desplazamientoX, posicionOriginal.y + desplazamientoY, posicionOriginal.z);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        // Volver a la posición original después del temblor
        transform.localPosition = posicionOriginal;
    }
}
    