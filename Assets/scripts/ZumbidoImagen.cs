using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZumbidoImagen : MonoBehaviour
{
    public Image imagen;          // Referencia a la imagen dentro del Canvas
    public float radio = 10f;     // Radio del círculo en el que la imagen se moverá
    public float velocidad = 5f;  // Velocidad de la animación (cuán rápido se moverá en el círculo)

    private Vector3 posicionOriginal;  // Posición original de la imagen
    private RectTransform rectTransform; // Referencia al RectTransform
    private float angulo = 0f;    // Ángulo actual en el círculo (en radianes)
    private bool movimientoActivo = false; // Control de si el movimiento circular está activo

    private void Start()
    {
        if (imagen != null)
        {
            // Obtener el RectTransform de la imagen en el Canvas
            rectTransform = imagen.rectTransform;

            // Guarda la posición original de la imagen en el Canvas
            posicionOriginal = rectTransform.anchoredPosition;
            IniciarMovimientoCircular();
        }
    }

    public void IniciarMovimientoCircular()
    {
        if (imagen != null && !movimientoActivo)
        {
            movimientoActivo = true;
            StartCoroutine(MoverEnCirculo());
        }
    }

    public void DetenerMovimientoCircular()
    {
        if (imagen != null && movimientoActivo)
        {
            movimientoActivo = false;
            StopCoroutine(MoverEnCirculo());

            // Restaurar la posición original cuando se detiene el movimiento
            rectTransform.anchoredPosition = posicionOriginal;
        }
    }

    private IEnumerator MoverEnCirculo()
    {
        while (movimientoActivo)
        {
            // Calcular la nueva posición en el círculo usando trigonometría
            float x = Mathf.Cos(angulo) * radio;
            float y = Mathf.Sin(angulo) * radio;

            // Asignar la nueva posición al RectTransform de la imagen
            rectTransform.anchoredPosition = posicionOriginal + new Vector3(x, y, 0);

            // Incrementar el ángulo en función de la velocidad (ajustado por Time.deltaTime para suavidad)
            angulo += velocidad * Time.deltaTime;

            // Mantener el ángulo dentro de 0 a 2π (360 grados)
            if (angulo >= Mathf.PI * 2)
            {
                angulo = 0f;
            }

            // Esperar un pequeño intervalo antes del siguiente paso
            yield return null;
        }
    }
}