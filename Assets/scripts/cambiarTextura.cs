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
        SceneManager.LoadScene("Main");
    }
}
