using UnityEngine;

[CreateAssetMenu(fileName = "LuchadorData", menuName = "Torneo/LuchadorData")]
public class LuchadorData : ScriptableObject
{
    public string nombre;
    [Range(0, 100)] public float fuerza;
    [Range(0, 100)] public float velocidad;
    [Range(0, 100)] public float resistencia;
    public float vidaMaxima = 100f;
    public Texture2D imagen; 
}
