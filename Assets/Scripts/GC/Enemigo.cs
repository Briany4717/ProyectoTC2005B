using UnityEngine;


/// Representa la información y comportamiento base de un enemigo.

[System.Serializable]
public class Enemigo
{
    public string nombre;
    public int camaraActual;
    public float tiempoMovimiento;
    public bool eliminado = false;
    public GameObject prefabUI;
    public Vector2[] posicionesPorCamara;
    public float[] escalasPorCamara;
    public float tiempoMovimientoOriginal;
}