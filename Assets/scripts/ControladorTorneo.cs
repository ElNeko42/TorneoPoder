using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControladorTorneo : MonoBehaviour
{
    public LuchadorData[] todosLuchadores; // Datos de todos los luchadores del torneo
    public Transform[] respawnPoints; // Solo dos puntos de respawn
    public GameObject posicionLuchaluchador1;
    public GameObject posicionLuchaluchador2;
    public TextMeshProUGUI textoRonda; // UI para mostrar la ronda actual
    public TextMeshProUGUI textoCombate; // UI para mostrar qué luchadores están peleando
    public TextMeshProUGUI textoEventos; // UI para mostrar los eventos del combate
    public GameObject luchadorPrefab; // Asigna el prefab de tu luchador en el inspector
    private List<LuchadorData> luchadoresRestantes; // Lista dinámica de datos de luchadores que quedan en el torneo
    private int rondaActual = 1;

    void Start()
    {
        luchadoresRestantes = new List<LuchadorData>(todosLuchadores); // Inicia la lista de datos de luchadores
        StartCoroutine(PrepararTorneo());
    }

    // Método para mostrar eventos en pantalla
    public void MostrarEvento(string mensaje)
    {
        textoEventos.text += mensaje + "\n";
    }

    // Corrutina para manejar las rondas del torneo
    private IEnumerator PrepararTorneo()
    {
        while (luchadoresRestantes.Count > 1)
        {
            textoEventos.text = ""; // Limpiar los eventos de la ronda anterior
            textoRonda.text = "Ronda " + rondaActual;

            List<(LuchadorData, LuchadorData)> paresDeLuchadores = GenerarCuadroDeCombate(luchadoresRestantes);

            foreach (var par in paresDeLuchadores)
            {
                textoCombate.text = par.Item1.nombre + " vs " + (par.Item2 != null ? par.Item2.nombre : "Automático");

                if (par.Item2 != null)
                {
                    yield return StartCoroutine(PrepararCombate(par.Item1, par.Item2));
                }
                else
                {
                    luchadoresRestantes.Add(par.Item1);
                    MostrarEvento($"{par.Item1.nombre} avanza automáticamente!");
                    yield return new WaitForSeconds(2f);
                }
            }

            rondaActual++;
        }

        textoRonda.text = "¡El ganador del torneo es " + luchadoresRestantes[0].nombre + "!";
    }

    private List<(LuchadorData, LuchadorData)> GenerarCuadroDeCombate(List<LuchadorData> luchadores)
    {
        List<(LuchadorData, LuchadorData)> cuadro = new List<(LuchadorData, LuchadorData)>();
        System.Random random = new System.Random();
        for (int i = 0; i < luchadores.Count; i++)
        {
            int randomIndex = random.Next(i, luchadores.Count);
            LuchadorData temp = luchadores[i];
            luchadores[i] = luchadores[randomIndex];
            luchadores[randomIndex] = temp;
        }

        for (int i = 0; i < luchadores.Count; i += 2)
        {
            if (i + 1 < luchadores.Count)
            {
                cuadro.Add((luchadores[i], luchadores[i + 1]));
            }
            else
            {
                cuadro.Add((luchadores[i], null));
            }
        }

        luchadoresRestantes.Clear();

        return cuadro;
    }

    private IEnumerator PrepararCombate(LuchadorData luchador1Data, LuchadorData luchador2Data)
    {
        // Instanciar los luchadores en los puntos de respawn
        GameObject luchadorObj1 = Instantiate(luchadorPrefab, respawnPoints[0].position, Quaternion.identity);
        GameObject luchadorObj2 = Instantiate(luchadorPrefab, respawnPoints[1].position, Quaternion.identity);

        Luchador luchadorInstancia1 = luchadorObj1.GetComponent<Luchador>();
        Luchador luchadorInstancia2 = luchadorObj2.GetComponent<Luchador>();

        // Asignarles los datos del ScriptableObject
        luchadorInstancia1.luchadorData = luchador1Data;
        luchadorInstancia2.luchadorData = luchador2Data;

        // Asignar referencia al controlador
        luchadorInstancia1.controladorTorneo = this;
        luchadorInstancia2.controladorTorneo = this;

        // Asignar posiciones de ataque
        luchadorInstancia1.posicionAtaque = posicionLuchaluchador1.transform.position;
        luchadorInstancia2.posicionAtaque = posicionLuchaluchador2.transform.position;

        yield return new WaitForSeconds(2f);

        // Determinar quién ataca primero basado en la velocidad
        bool turnoLuchador1 = luchador1Data.velocidad >= luchador2Data.velocidad;

        // Ciclo de combate
        while (luchadorInstancia1.EstaVivo() && luchadorInstancia2.EstaVivo())
        {
            if (turnoLuchador1)
            {
                // Luchador 1 ataca
                yield return StartCoroutine(luchadorInstancia1.Atacar(luchadorInstancia2));
            }
            else
            {
                // Luchador 2 ataca
                yield return StartCoroutine(luchadorInstancia2.Atacar(luchadorInstancia1));
            }

            // Cambiar de turno
            turnoLuchador1 = !turnoLuchador1;

            // Esperar un tiempo antes del siguiente ataque
            yield return new WaitForSeconds(1f);
        }

        // Determinar el ganador y mostrar mensaje
        if (!luchadorInstancia1.EstaVivo())
        {
            luchadoresRestantes.Add(luchador2Data);
            MostrarEvento($"{luchador2Data.nombre} avanza al siguiente round!");
        }
        else
        {
            luchadoresRestantes.Add(luchador1Data);
            MostrarEvento($"{luchador1Data.nombre} avanza al siguiente round!");
        }

        // Destruir los objetos una vez que termina la pelea
        Destroy(luchadorObj1);
        Destroy(luchadorObj2);

        yield return null;
    }
}
