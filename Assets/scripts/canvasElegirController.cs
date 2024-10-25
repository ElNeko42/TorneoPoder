using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class canvasElegirController : MonoBehaviour
{
    public Canvas canvasMenu;
    public Canvas canvasElegir;
    public Canvas canvasTutorial;
    public CicloLuchadores cicloLuchadores;
    public Button botonElegir;
    private ElegirPersonaje elegirPersonaje;
    private void Awake()
    {
        elegirPersonaje = FindAnyObjectByType<ElegirPersonaje>();
    }

    public void Atras()
    {
        // Verificar si el cubo instanciado existe y destruirlo
        if (ElegirPersonaje.cuboInstanciado != null)
        {
            Destroy(ElegirPersonaje.cuboInstanciado);
            ElegirPersonaje.cuboInstanciado = null; // Limpiar la referencia
        }

        // Cambiar de canvas
        canvasMenu.enabled = true;
        canvasElegir.enabled = false;
        GameManager.instance.jugadorActual = null;
        Button botonJugar = GameObject.FindGameObjectWithTag("Jugar").GetComponent<Button>();
        botonJugar.interactable = false;
    }

    public void Elegir()
    {
        if (GameManager.instance.jugadorActual != null)
        {
            GameManager.instance.esFinal = false;
            SceneManager.LoadScene("VSScene");
        }
    }

    public void Abrir()
    {
        canvasElegir.enabled = true;
        canvasMenu.enabled = false;
        cicloLuchadores.Parar();
    }

    public void Tutorial()
    {
        canvasTutorial.enabled = true;
        canvasMenu.enabled = false;
    }

    public void CerrarTutorial()
    {
        canvasTutorial.enabled = false;
        canvasMenu.enabled = true;
    }
}
