using UnityEngine;

public class BolaEnergia : MonoBehaviour
{
    private Luchador oponente;
    private float daño;
    public float velocidad = 10f; // Velocidad de la bola de energía

    public void Inicializar(Luchador oponente, float daño)
    {
        this.oponente = oponente;
        this.daño = daño;
    }

    void Update()
    {
        if (oponente != null)
        {
            // Direccionar la bola hacia el oponente
            Vector3 direccion = (oponente.transform.position - transform.position).normalized;

            // Mover la bola hacia el oponente
            transform.position += direccion * velocidad * Time.deltaTime;

            // Opcional: hacer que la bola mire hacia el oponente (si quieres un efecto visual de rotación)
            transform.LookAt(oponente.transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == oponente.gameObject)
        {
            //oponente.RecibirGolpe(daño, Vector3.zero,true);
            Destroy(gameObject); // Destruir la bola de energía
        }
    }
}
