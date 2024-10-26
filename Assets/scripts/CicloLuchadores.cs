using System.Collections;
using UnityEngine;

public class CicloLuchadores : MonoBehaviour
{
    public GameObject luchadorPrefab; // Prefab del luchador
    public LuchadorData[] listaLuchadores; // Array de ScriptableObjects de luchadores
    public Transform puntoDeRespawn; // Punto elevado desde donde caen los luchadores
    public float intervaloEntreLuchadores = 3f; // Tiempo entre la aparición de luchadores
    public float tiempoAntesDeDestruir = 2f; // Tiempo antes de que se destruya el luchador
    public float alturaCaida = 10f; // Altura desde la que caen los luchadores
    public float fuerzaEmpuje = 2f; // Fuerza de empuje cuando tocan el suelo
    public float tiempoDeformacion = 0.2f; // Duración de la deformación "goofy"
    public float escalaDeformacion = 1.5f; // Escala exagerada durante la deformación

    private int indiceLuchadorActual = 0; // Índice del luchador que se va a instanciar

    void Start()
    {
        StartCoroutine(CicloAparicionLuchadores());
    }

    private IEnumerator CicloAparicionLuchadores()
    {
        while (true)
        {
            // Instanciar un nuevo luchador en un punto elevado
            Vector3 puntoInicial = puntoDeRespawn.position + Vector3.up * alturaCaida;
            GameObject luchadorInstancia = Instantiate(luchadorPrefab, puntoInicial, Quaternion.identity);

            // Asignar los datos del ScriptableObject al luchador
            Luchador luchadorScript = luchadorInstancia.GetComponent<Luchador>();
            LuchadorData luchadorDataActual = listaLuchadores[indiceLuchadorActual];
            luchadorScript.luchadorData = luchadorDataActual;

            // Aumentar el índice del luchador actual (circular)
            indiceLuchadorActual = (indiceLuchadorActual + 1) % listaLuchadores.Length;

            // Aplicar la textura del LuchadorData al modelo
            Renderer renderer = luchadorInstancia.GetComponent<Renderer>();
            if (renderer != null && luchadorDataActual.imagen != null)
            {
                // Clonar el material para evitar modificar el material original
                renderer.material = new Material(renderer.material);
                renderer.material.mainTexture = luchadorDataActual.imagen;
            }
            else
            {
                Debug.LogWarning("No se pudo asignar la textura al luchador. Verifica que el prefab tenga un Renderer y que la textura esté asignada en LuchadorData.");
            }

            // Aplicar gravedad para la caída
            Rigidbody rb = luchadorInstancia.GetComponent<Rigidbody>();
            rb.useGravity = true;

            // Esperar a que el luchador toque el suelo
            yield return new WaitForSeconds(0.5f); // Ajusta según la altura de la caída

            // Realizar deformación "goofy" al tocar el suelo
            yield return StartCoroutine(DeformarLuchador(luchadorInstancia.transform));

            // Empujar al luchador en una dirección aleatoria
            Vector3 direccionAleatoria = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            rb.AddForce(direccionAleatoria * fuerzaEmpuje, ForceMode.Impulse);

            // Esperar antes de destruir el luchador
            yield return new WaitForSeconds(tiempoAntesDeDestruir);

            // Destruir el luchador
            Destroy(luchadorInstancia);

            // Esperar el intervalo antes de crear el siguiente luchador
            yield return new WaitForSeconds(intervaloEntreLuchadores);
        }
    }

    // Corrutina para deformar el luchador de forma "goofy" al aterrizar
    private IEnumerator DeformarLuchador(Transform luchadorTransform)
    {
        Vector3 escalaOriginal = luchadorTransform.localScale;

        // Asignar la escala deformada correctamente
        Vector3 escalaDeformada = new Vector3(escalaOriginal.x * escalaDeformacion, escalaOriginal.y * 0.5f, escalaOriginal.z * escalaDeformacion);

        // Estirar al caer
        float tiempo = 0;
        while (tiempo < tiempoDeformacion)
        {
            luchadorTransform.localScale = Vector3.Lerp(escalaOriginal, escalaDeformada, tiempo / tiempoDeformacion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        // Deformar al tocar el suelo
        tiempo = 0;
        while (tiempo < tiempoDeformacion)
        {
            luchadorTransform.localScale = Vector3.Lerp(escalaDeformada, escalaOriginal, tiempo / tiempoDeformacion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        luchadorTransform.localScale = escalaOriginal; // Restaurar la escala original
    }

    public void Parar()
    {
        StopAllCoroutines();
    }
    public void Reanudar()
    {
        StartCoroutine(CicloAparicionLuchadores());
    }
}
