using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PerderController : MonoBehaviour
{
    // Start is called before the first frame update
    public int tiempo = 10;
    public TextMeshProUGUI textoTiempo;
    
    void Start()
    {
        StartCoroutine(Perder());        
    }

    private IEnumerator Perder()
    {
        while (tiempo > -1)
        {
            textoTiempo.text = tiempo.ToString();
            yield return new WaitForSeconds(1);
            tiempo--;
        }
        SceneManager.LoadScene("Main");
    }

    public void VolverMenu()
    {
        SceneManager.LoadScene("Main");
    }

    public void VolverTorneo()
    {
        //TournamentData.rondaActual--;
        SceneManager.LoadScene("CombateScene");
    }
}
