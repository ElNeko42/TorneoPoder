using UnityEngine;

public class BolaEnergia : MonoBehaviour
{
    private Luchador oponente;
    private float daño;

    public void Inicializar(Luchador oponente, float daño)
    {
        this.oponente = oponente;
        this.daño = daño;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == oponente.gameObject)
        {
            oponente.RecibirGolpe(daño, Vector3.zero);
            Destroy(gameObject); // Destruir la bola de energía
        }
    }
}
