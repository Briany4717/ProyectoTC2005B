using UnityEngine;


/// Detecta cuando el jugador alcanza el punto final del minijuego de control.

public class GLFinalPoint : MonoBehaviour
{
    [SerializeField] StationData stationToComplete;

    
    /// Completa la estación actual al colisionar con el objeto verificador.
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GLChecker"))
        {
            Debug.Log("Llegaste al final!!!");
            GLMenusStationsManager.Instance.CloseAllMenus();
            OrderManager.Instance.OnPlayerCompletedStation(stationToComplete);
        }
    }
}