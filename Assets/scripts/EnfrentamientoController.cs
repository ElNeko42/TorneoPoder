using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EnfrentamientoController : MonoBehaviour
{
    public List<LuchadorData> todosLuchadores; // Lista de todos los luchadores disponibles
    public LuchadorData jugador; // El jugador
    private List<LuchadorData> oponentes; // Lista de oponentes generados para el torneo
    private int oponenteActual = 0; // �ndice del oponente actual

    public RawImage imagenJugador1;
    public RawImage imagenJugador2;
    public TextMeshProUGUI nombreRonda;
    public TextMeshProUGUI nombreJugador1;
    public TextMeshProUGUI nombreJugador2;

    private void Awake()
    {
        jugador = GameManager.instance.jugadorActual;
    }
    void Start()
    {
        if (GameManager.instance.rondaActual == 0)
        {
            // Generar el torneo al inicio si no se ha generado a�n
            GenerarTorneo();
            // Mostrar datos del enfrentamiento actual
            MostrarEnfrentamiento();
            // Esperar unos segundos y luego pasar a la escena de combate
            StartCoroutine(EsperarYCargarCombate());
        }
        else 
        {
            Debug.Log("Ronda actual: " + GameManager.instance.rondaActual);
            oponenteActual = GameManager.instance.rondaActual;
            Debug.Log("Oponente actual: " + oponenteActual);
            SiguienteEnfrentamiento();
        }


    }

    void GenerarTorneo()
    {
        oponentes = new List<LuchadorData>();
        for (int i = 0; i < 4; i++)
        {
            LuchadorData oponente;
            do
            {
                oponente = todosLuchadores[Random.Range(0, todosLuchadores.Count)];
            } while (oponente == jugador || oponentes.Contains(oponente));

            oponentes.Add(oponente);
        }

        // Guardar los datos generados en TournamentData
        TournamentData.jugadorActual = jugador;
        TournamentData.luchadoresRestantes = oponentes;
        TournamentData.rondaActual = 1;
    }

    void MostrarEnfrentamiento()
    {
        // Mostrar la imagen y el nombre del jugador
        imagenJugador1.texture = TournamentData.jugadorActual.imagen;
        nombreJugador1.text = TournamentData.jugadorActual.nombre;

        // Mostrar la imagen y el nombre del oponente actual
        imagenJugador2.texture = TournamentData.luchadoresRestantes[oponenteActual].imagen;
        nombreJugador2.text = TournamentData.luchadoresRestantes[oponenteActual].nombre;

        // Mostrar la ronda actual
        nombreRonda.text = "Round " + TournamentData.rondaActual;
    }

    private IEnumerator EsperarYCargarCombate()
    {
        Debug.Log("Oponentes" + oponentes);

        Debug.Log("Oponente actual: " + oponenteActual);

        yield return new WaitForSeconds(3f); // Esperar 3 segundos

        // Asignar el oponente actual a TournamentData
        TournamentData.oponenteActual = TournamentData.luchadoresRestantes[oponenteActual];

        // Aqu� actualizamos el GameManager con los datos del jugador y del oponente actual
        GameManager.instance.ConfigurarLuchadores(TournamentData.jugadorActual, TournamentData.oponenteActual);
        GameManager.instance.rondaActual = TournamentData.rondaActual;

        // Cargar la escena de combate
        SceneManager.LoadScene("CombateScene");
    }

    // M�todo para continuar con el siguiente enfrentamiento o mostrar la victoria
    public void SiguienteEnfrentamiento()
    {
        // Verificar si este es el �ltimo combate
        if (oponenteActual + 1 >= TournamentData.luchadoresRestantes.Count)
        {
            // Este es el �ltimo combate; no hay m�s combates despu�s
            GameManager.instance.esFinal = true;
        }
        else
        {
            // Todav�a hay m�s combates despu�s de este
            GameManager.instance.esFinal = false;
        }

        // Preparar el siguiente enfrentamiento
        //oponenteActual++;
        TournamentData.rondaActual++;
        MostrarEnfrentamiento();
        StartCoroutine(EsperarYCargarCombate());
    }


}
