using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControladorTorneo : MonoBehaviour
{
    public LuchadorData[] todosLuchadores; // Datos de todos los luchadores del torneo
    public Transform[] respawnPoints; // Solo dos puntos de respawn
    public TextMeshProUGUI textoRonda; // UI para mostrar la ronda actual
    public TextMeshProUGUI textoCombate; // UI para mostrar qué luchadores están peleando
    public GameObject luchadorPrefab; // Asigna el prefab de tu luchador en el inspector

    private List<LuchadorData> luchadoresRestantes; // Lista dinámica de datos de luchadores que quedan en el torneo
    private int rondaActual = 1;

    void Start()
    {
        luchadoresRestantes = new List<LuchadorData>(todosLuchadores); // Inicia la lista de datos de luchadores
        StartCoroutine(PrepararTorneo());
    }

    // Corrutina para manejar las rondas del torneo
    private IEnumerator PrepararTorneo()
    {
        while (luchadoresRestantes.Count > 1)
        {
            // Mostrar la ronda actual
            textoRonda.text = "Ronda " + rondaActual;

            // Emparejar a los luchadores para esta ronda
            List<(LuchadorData, LuchadorData)> paresDeLuchadores = GenerarCuadroDeCombate(luchadoresRestantes);

            foreach (var par in paresDeLuchadores)
            {
                // Mostrar qué luchadores van a pelear
                textoCombate.text = par.Item1.nombre + " vs " + (par.Item2 != null ? par.Item2.nombre : "Automático");

                // Realizar la pelea si ambos luchadores existen
                if (par.Item2 != null)
                {
                    yield return StartCoroutine(PrepararCombate(par.Item1, par.Item2));
                }
                else
                {
                    // Si no hay un segundo luchador, el primero avanza automáticamente
                    luchadoresRestantes.Add(par.Item1);
                    textoCombate.text = par.Item1.nombre + " avanza automáticamente!";
                    yield return new WaitForSeconds(2f); // Pausa de 2 segundos
                }
            }

            // Incrementar la ronda y reducir el número de luchadores restantes
            rondaActual++;
        }

        // Final del torneo
        textoRonda.text = "¡El ganador del torneo es " + luchadoresRestantes[0].nombre + "!";
    }

    // Función para emparejar luchadores y generar el cuadro de combate
    private List<(LuchadorData, LuchadorData)> GenerarCuadroDeCombate(List<LuchadorData> luchadores)
    {
        List<(LuchadorData, LuchadorData)> cuadro = new List<(LuchadorData, LuchadorData)>();

        // Mezclar la lista de luchadores aleatoriamente
        System.Random random = new System.Random();
        for (int i = 0; i < luchadores.Count; i++)
        {
            int randomIndex = random.Next(i, luchadores.Count);
            LuchadorData temp = luchadores[i];
            luchadores[i] = luchadores[randomIndex];
            luchadores[randomIndex] = temp;
        }

        // Emparejar luchadores de dos en dos
        for (int i = 0; i < luchadores.Count; i += 2)
        {
            if (i + 1 < luchadores.Count)
            {
                cuadro.Add((luchadores[i], luchadores[i + 1]));
            }
            else
            {
                cuadro.Add((luchadores[i], null)); // Si hay un luchador sin oponente
            }
        }

        // Limpiar la lista de luchadores restantes para la siguiente ronda
        luchadoresRestantes.Clear();

        return cuadro;
    }

    // Corrutina para manejar el combate entre dos luchadores
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

        // Esperar unos segundos antes de comenzar la pelea
        yield return new WaitForSeconds(2f);

        // Iniciar el combate
        luchadorInstancia1.Atacar(luchadorInstancia2);
        yield return new WaitUntil(() => !luchadorInstancia1.EstaVivo() || !luchadorInstancia2.EstaVivo());

        // Determinar el ganador
        if (!luchadorInstancia1.EstaVivo())
        {
            luchadoresRestantes.Add(luchador2Data);
        }
        else
        {
            luchadoresRestantes.Add(luchador1Data);
        }

        // Destruir los objetos una vez que termina la pelea
        Destroy(luchadorObj1);
        Destroy(luchadorObj2);
    }
}
