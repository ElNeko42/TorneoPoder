using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EleegirController : MonoBehaviour
{
    public Canvas canvas;
    public Sprite punetazo;
    public Sprite especial;
    public Sprite esquivar;
    public TextMeshProUGUI textoKi;
    public RawImage[] slotsAcciones;
    public int ki = 5;
    public int costeAtaque = 1;
    public int costeEspecial = 3;
    public int costeEsquivar = 1;

    public List<string> accionesSeleccionadas = new List<string>();
    public bool accionesListas = false;
    private AudioSource audioSourceEfectos;
    public AudioClip sonidoEscoger;

    private void Awake()
    {
        foreach (RawImage slot in slotsAcciones)
        {
            slot.enabled = false;
            slot.texture = null;
        }
        // Buscar el GameObject con el tag "efectos" para asignar audioSourceEfectos
        GameObject efectosObj = GameObject.FindGameObjectWithTag("efectos");
        if (efectosObj != null)
        {
            audioSourceEfectos = efectosObj.GetComponent<AudioSource>();
            if (audioSourceEfectos == null)
            {
                Debug.LogError("No se encontró un AudioSource en el GameObject con el tag 'efectos'.");
            }
        }
        else
        {
            Debug.LogError("No se encontró ningún GameObject con el tag 'efectos'.");
        }
        textoKi.text = "Ki: " + ki + "/" + GameManager.instance.jugadorActual.energiaMaxima;
    }

    private void Update()
    {
        if (canvas.enabled)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                ElegirAccion(0);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                ElegirAccion(1);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                ElegirAccion(2);
            }
        }
    }

    private void ElegirAccion(int v)
    {
        int coste = 0;
        Sprite accionSprite = null;
        string accionNombre = "";

        switch (v)
        {
            case 0:
                coste = costeAtaque;
                accionSprite = punetazo;
                accionNombre = "pegar";
                break;
            case 1:
                coste = costeEspecial;
                accionSprite = especial;
                accionNombre = "especial";
                break;
            case 2:
                coste = costeEsquivar;
                accionSprite = esquivar;
                accionNombre = "esquivar";
                break;
            default:
                return; // Acción inválida
        }

        // Verificar si hay suficiente ki
        if (ki < coste)
        {
            // No hay suficiente ki
            return;
        }
        // Encontrar el siguiente slot disponible
        int slotIndex = -1;
        for (int i = 0; i < slotsAcciones.Length; i++)
        {
            if (!slotsAcciones[i].enabled)
            {
                slotIndex = i;
                break;
            }
        }

        if (slotIndex == -1)
        {
            // No hay slots disponibles
            return;
        }

        // Colocar la imagen en el slot
        slotsAcciones[slotIndex].texture = accionSprite.texture;
        slotsAcciones[slotIndex].enabled = true;

        // Deducir el ki
        ki -= coste;

        // Actualizar el texto de ki
        ActualizarTextoKi();
        audioSourceEfectos.PlayOneShot(sonidoEscoger);
        // Agregar la acción seleccionada a la lista
        accionesSeleccionadas.Add(accionNombre);
    }

    public void ResetearAcciones()
    {
        foreach (RawImage slot in slotsAcciones)
        {
            slot.enabled = false;
            slot.texture = null;
        }
        accionesSeleccionadas.Clear();
        ki = GameManager.instance.jugadorActual.energiaMaxima;
        ActualizarTextoKi();
    }

    public void MostrarCanvas()
    {
        ki = GameManager.instance.jugadorActual.energiaMaxima;
        canvas.enabled = true;
        ActualizarTextoKi();
    }

    public void ActualizarTextoKi()
    {
        textoKi.text = "Ki: " + ki + "/" + GameManager.instance.jugadorActual.energiaMaxima;
    }

    public void Luchar()
    {
        canvas.enabled = false;
        accionesListas = true;
    }
}
