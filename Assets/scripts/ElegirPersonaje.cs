using UnityEngine;
using UnityEngine.UI;

public class ElegirPersonaje : MonoBehaviour
{
    public LuchadorData luchador;
    public GameObject cuboPrefab;
    public Camera personajeCamera; // Referencia a la c�mara "PersonajeCamera"
    public static GameObject cuboInstanciado; 
    public Transform puntoDeRespawn; // Punto desde donde aparece el cubo

    public void Elegir()
    {
        GameManager.instance.jugadorActual = luchador;

        // Destruir el cubo anterior si existe
        if (cuboInstanciado != null)
        {
            Destroy(cuboInstanciado);
        }

        // Instanciar el cubo en el punto de respawn
        cuboInstanciado = Instantiate(cuboPrefab, puntoDeRespawn.position, puntoDeRespawn.rotation);

        // Asignar el cubo a la capa "Personaje3D"
        cuboInstanciado.layer = LayerMask.NameToLayer("Personaje3D");

        // Desactivar la gravedad en el Rigidbody
        Rigidbody rb = cuboInstanciado.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true; // Hacer que el cubo no sea afectado por f�sicas
        }
        else
        {
            Debug.LogError("El prefab del cubo no tiene un componente Rigidbody.");
        }

        // Aplicar el material con el sprite del personaje
        Renderer renderer = cuboInstanciado.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material nuevoMaterial = new Material(renderer.material);
            nuevoMaterial.mainTexture = luchador.imagen;
            renderer.material = nuevoMaterial;
        }
        else
        {
            Debug.LogError("El prefab del cubo no tiene un componente Renderer.");
        }

        // A�adir el script de rotaci�n al cubo
        cuboInstanciado.AddComponent<RotacionCubo>();

        // Habilitar el bot�n "Jugar"
        Button botonJugar = GameObject.FindGameObjectWithTag("Jugar").GetComponent<Button>();
        botonJugar.interactable = true;
    }
}
