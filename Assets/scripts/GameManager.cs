using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Datos del jugador y del oponente
    public LuchadorData jugadorActual;
    public LuchadorData oponenteActual;
    public int rondaActual = 0;
    private AudioSource audioSourceEfectos;
    public bool esFinal=false;

    private void Awake()
    {
        // Asegurar que solo exista una instancia de GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Si ya existe, destruir el nuevo
        }
    }

    // Método para configurar los luchadores
    public void ConfigurarLuchadores(LuchadorData jugador, LuchadorData oponente)
    {
        jugadorActual = jugador;
        oponenteActual = oponente;
    }


}
