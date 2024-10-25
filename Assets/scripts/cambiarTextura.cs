using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cambiarTextura : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.mainTexture = GameManager.instance.jugadorActual.imagen;
    }

    public void volverMenu()
    {
        TournamentData.rondaActual = 0;
        GameManager.instance.jugadorActual = null;
        GameManager.instance.esFinal = false;
        GameManager.instance.oponenteActual = null;
        GameManager.instance.rondaActual = 0;
        SceneManager.LoadScene("Main");
    }
}
