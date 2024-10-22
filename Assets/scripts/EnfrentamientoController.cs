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
    private int oponenteActual = 0; // Índice del oponente actual

    public RawImage imagenJugador1;
    public RawImage imagenJugador2;
    public TextMeshProUGUI nombreRonda;
    public TextMeshProUGUI nombreJugador1;
    public TextMeshProUGUI nombreJugador2;

    void Start()
    {
        if (GameManager.instance.rondaActual == 0)
        {
            Debug.Log("Ronda actual0: " + GameManager.instance.rondaActual);
            if (TournamentData.luchadoresRestantes.Count == 0)
            {
                // Generar el torneo al inicio si no se ha generado aún
                GenerarTorneo();
            }
        }
        else 
        {
            Debug.Log("Ronda actual: " + GameManager.instance.rondaActual);
            oponenteActual = GameManager.instance.rondaActual;
            Debug.Log("Oponente actual: " + oponenteActual);
            SiguienteEnfrentamiento();
        }

        // Mostrar datos del enfrentamiento actual
        MostrarEnfrentamiento();

        // Esperar unos segundos y luego pasar a la escena de combate
        StartCoroutine(EsperarYCargarCombate());
    }

    void GenerarTorneo()
    {
        oponentes = new List<LuchadorData>();

        // Seleccionamos 4 oponentes al azar excluyendo al jugador
        for (int i = 0; i < 4; i++)
        {
            LuchadorData oponente = todosLuchadores[Random.Range(0, todosLuchadores.Count)];
            if (!oponentes.Contains(oponente) && oponente != jugador)
            {
                oponentes.Add(oponente);
            }
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

        // Aquí actualizamos el GameManager con los datos del jugador y del oponente actual
        GameManager.instance.ConfigurarLuchadores(TournamentData.jugadorActual, TournamentData.oponenteActual);
        GameManager.instance.rondaActual = TournamentData.rondaActual;

        // Cargar la escena de combate
        SceneManager.LoadScene("CombateScene");
    }

    // Método para continuar con el siguiente enfrentamiento o mostrar la victoria
    public void SiguienteEnfrentamiento()
    {
        if (oponenteActual < TournamentData.luchadoresRestantes.Count)
        {
            // Avanzar a la siguiente ronda y actualizar la información del torneo
            TournamentData.rondaActual++;
            EsperarYCargarCombate();
        }
        else
        {
            // El jugador ha ganado el torneo, mostrar la escena de victoria
            SceneManager.LoadScene("VictoriaScene");
        }
    }
}
