using UnityEngine;

[System.Serializable]
public class Enemigo
{
    public string nombre;
    public int camaraActual;
    public float tiempoMovimiento;
    public bool eliminado = false;
    public GameObject prefabUI;
    public Vector2[] posicionesPorCamara;
    public float[] escalasPorCamara; // 1 = normal, 0.5 = lejos, 1.5 = cerca
    public float tiempoMovimientoOriginal;
}