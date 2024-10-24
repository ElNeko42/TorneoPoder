using UnityEngine;

public class BolaEnergia : MonoBehaviour
{
    private Luchador oponente;
    private float da�o;
    public float velocidad = 10f; // Velocidad de la bola de energ�a

    public void Inicializar(Luchador oponente, float da�o)
    {
        this.oponente = oponente;
        this.da�o = da�o;
    }

    void Update()
    {
        if (oponente != null)
        {
            // Direccionar la bola hacia el oponente
            Vector3 direccion = (oponente.transform.position - transform.position).normalized;

            // Mover la bola hacia el oponente
            transform.position += direccion * velocidad * Time.deltaTime;

            // Opcional: hacer que la bola mire hacia el oponente (si quieres un efecto visual de rotaci�n)
            transform.LookAt(oponente.transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == oponente.gameObject)
        {
            //oponente.RecibirGolpe(da�o, Vector3.zero,true);
            Destroy(gameObject); // Destruir la bola de energ�a
        }
    }
}
