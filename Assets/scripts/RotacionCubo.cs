using UnityEngine;

public class RotacionCubo : MonoBehaviour
{
    public Vector3 velocidadRotacion = new Vector3(30f, 45f, 60f); // Velocidad de rotación en grados por segundo

    void Update()
    {
        transform.Rotate(velocidadRotacion * Time.deltaTime);
    }
}
