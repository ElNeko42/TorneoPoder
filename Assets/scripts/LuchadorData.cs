using UnityEngine;

[CreateAssetMenu(fileName = "LuchadorData", menuName = "Torneo/LuchadorData")]
public class LuchadorData : ScriptableObject
{
    public string nombre;
    [Range(0, 100)] public int fuerza;
    [Range(0, 100)] public int velocidad;
    [Range(0, 100)] public int resistencia;
    public float vidaMaxima = 100f;
}
