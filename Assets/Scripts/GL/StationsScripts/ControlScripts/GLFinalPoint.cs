using UnityEngine;

public class GLFinalPoint : MonoBehaviour
{
    [SerializeField] StationData stationToComplete;
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
