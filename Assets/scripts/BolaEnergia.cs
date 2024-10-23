using UnityEngine;

public class BolaEnergia : MonoBehaviour
{
    private Luchador oponente;
    private float da�o;

    public void Inicializar(Luchador oponente, float da�o)
    {
        this.oponente = oponente;
        this.da�o = da�o;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == oponente.gameObject)
        {
            oponente.RecibirGolpe(da�o, Vector3.zero);
            Destroy(gameObject); // Destruir la bola de energ�a
        }
    }
}
