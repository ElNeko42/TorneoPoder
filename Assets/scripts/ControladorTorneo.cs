using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MagicPigGames;

public class ControladorTorneo : MonoBehaviour
{
    public Transform[] respawnPoints; // Solo dos puntos de respawn
    public GameObject posicionLuchaluchador1;
    public GameObject posicionLuchaluchador2;
    public TextMeshProUGUI textoRonda; // UI para mostrar la ronda actual
    public TextMeshProUGUI textoCombate; // UI para mostrar qué luchadores están peleando
    public TextMeshProUGUI textoEventos; // UI para mostrar los eventos del combate
    public GameObject luchadorPrefab; // Prefab del luchador
    public GameObject ataqueEspecialPrefab;

    // Referencias a las barras de vida y las imágenes de los luchadores
    public RawImage imagenLuchador1Barra;
    public RawImage imagenLuchador2Barra;
    public ProgressBarInspectorTest barraLuchador1;
    public ProgressBarInspectorTest barraLuchador2;

    public EleegirController elegirController; // Asigna esto desde el inspector
    private List<string> movimientosJugador;

    // Referencias a las instancias de los luchadores en combate
    public Luchador luchadorInstancia1;
    public Luchador luchadorInstancia2;

    void Start()
    {
        StartCoroutine(PrepararTorneo());
    }

    // Método para mostrar eventos en pantalla
    public void MostrarEvento(string mensaje)
    {
        textoEventos.text = mensaje;
    }

    // Corrutina para manejar las rondas del torneo
    private IEnumerator PrepararTorneo()
    {
        // Configurar al jugador como el primer luchador
        LuchadorData jugadorData = GameManager.instance.jugadorActual;
        LuchadorData oponenteData = GameManager.instance.oponenteActual;

        // Preparar el combate con los datos del jugador y del oponente
        yield return StartCoroutine(PrepararCombate(jugadorData, oponenteData));

        // Mostrar mensaje de ganador
        textoRonda.text = "¡El ganador del torneo es " + jugadorData.nombre + "!";
    }

    private IEnumerator PrepararCombate(LuchadorData luchador1Data, LuchadorData luchador2Data)
    {
        // Instanciar los luchadores en los puntos de respawn
        GameObject luchadorObj1 = Instantiate(luchadorPrefab, respawnPoints[0].position, Quaternion.identity);
        GameObject luchadorObj2 = Instantiate(luchadorPrefab, respawnPoints[1].position, Quaternion.identity);

        luchadorInstancia1 = luchadorObj1.GetComponent<Luchador>();
        luchadorInstancia2 = luchadorObj2.GetComponent<Luchador>();

        // Asignarles los datos del ScriptableObject
        luchadorInstancia1.luchadorData = luchador1Data;
        luchadorInstancia2.luchadorData = luchador2Data;

        // Asignar referencia al controlador para ambos luchadores
        luchadorInstancia1.controladorTorneo = this;
        luchadorInstancia2.controladorTorneo = this;

        // Asignar posiciones de ataque
        luchadorInstancia1.posicionAtaque = posicionLuchaluchador1.transform.position;
        luchadorInstancia2.posicionAtaque = posicionLuchaluchador2.transform.position;

        // Asignar imágenes de los luchadores en las barras de vida
        if (imagenLuchador1Barra != null && luchador1Data.imagen != null)
        {
            imagenLuchador1Barra.texture = luchador1Data.imagen;
        }

        if (imagenLuchador2Barra != null && luchador2Data.imagen != null)
        {
            imagenLuchador2Barra.texture = luchador2Data.imagen;
        }

        // Inicializar las barras de vida
        ActualizarBarraVida(luchadorInstancia1);
        ActualizarBarraVida(luchadorInstancia2);

        yield return new WaitForSeconds(2f);

        // Ciclo de combate hasta que uno de los luchadores muera
        while (luchadorInstancia1.EstaVivo() && luchadorInstancia2.EstaVivo())
        {
            // Obtener los movimientos del jugador 1 (Luchador 1)
            yield return StartCoroutine(ObtenerMovimientosJugador());

            // Determinar quién ataca primero basado en la velocidad
            bool turnoLuchador1 = luchador1Data.velocidad >= luchador2Data.velocidad;

            // Índice para los movimientos del jugador
            int indiceMovimientoJugador = 0;

            // Mientras haya movimientos del jugador por ejecutar y ambos luchadores estén vivos
            while (indiceMovimientoJugador < movimientosJugador.Count && luchadorInstancia1.EstaVivo() && luchadorInstancia2.EstaVivo())
            {
                if (turnoLuchador1)
                {
                    // Luchador 1 ejecuta su acción
                    yield return StartCoroutine(EjecutarAccionJugador(movimientosJugador[indiceMovimientoJugador], luchadorInstancia1, luchadorInstancia2));
                    indiceMovimientoJugador++;
                }
                else
                {
                    // Luchador 2 ataca
                    yield return StartCoroutine(luchadorInstancia2.Atacar(luchadorInstancia1));
                }

                // Cambiar de turno
                turnoLuchador1 = !turnoLuchador1;

                // Esperar antes del siguiente turno
                yield return new WaitForSeconds(1f);
            }

            // Si los movimientos del jugador se han agotado pero el combate continúa, se sigue con el luchador 2
            while (luchadorInstancia1.EstaVivo() && luchadorInstancia2.EstaVivo() && !turnoLuchador1)
            {
                // Luchador 2 ataca
                yield return StartCoroutine(luchadorInstancia2.Atacar(luchadorInstancia1));

                // Cambiar de turno
                turnoLuchador1 = !turnoLuchador1;

                // Esperar antes del siguiente turno
                yield return new WaitForSeconds(1f);
            }
        }

        // Mostrar el ganador
        if (!luchadorInstancia1.EstaVivo())
        {
            
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("DerrotaScene");
        }
        else
        {
            yield return new WaitForSeconds(2f);
            if (GameManager.instance.esFinal)
            {
                SceneManager.LoadScene("VictoriaScene");
            }
            else
            { 
                SceneManager.LoadScene("VSScene");
            }
        }
    }

    private IEnumerator ObtenerMovimientosJugador()
    {
        // Reiniciar las acciones seleccionadas
        elegirController.ResetearAcciones();
        elegirController.accionesListas = false;

        // Mostrar el canvas para que el jugador seleccione sus acciones
        elegirController.MostrarCanvas();

        // Esperar hasta que el jugador presione "Luchar" y las acciones estén listas
        yield return new WaitUntil(() => elegirController.accionesListas);

        // Obtener la lista de acciones seleccionadas
        movimientosJugador = new List<string>(elegirController.accionesSeleccionadas);
    }

    private IEnumerator EjecutarAccionJugador(string accion, Luchador luchador1, Luchador luchador2)
    {
        if (accion == "pegar")
        {
            // El jugador ataca
            yield return StartCoroutine(luchador1.Atacar(luchador2));
        }
        else if (accion == "esquivar")
        {
            // El jugador esquiva
            MostrarEvento($"{luchador1.luchadorData.nombre} prepares to dodge.");
            luchador1.PrepararEsquivar();
            yield return new WaitForSeconds(1f);
        }
        else if (accion == "especial")
        {
            // El jugador realiza un ataque especial
            yield return StartCoroutine(luchador1.AtacarEspecial(luchador2));
        }
    }

    // Método para actualizar las barras de vida
    public void ActualizarBarraVida(Luchador luchador)
    {
        if (barraLuchador1 != null && luchador == luchadorInstancia1)
        {
            barraLuchador1.progress = luchador.GetHealthPercent();
        }

        if (barraLuchador2 != null && luchador == luchadorInstancia2)
        {
            barraLuchador2.progress = luchador.GetHealthPercent();
        }
    }
}
