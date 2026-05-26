using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public float seconds = 3, maxHeight = 10f;
    public string playerTag = "PNPlayer";
    public string buildingTag = "PNBuilding";

    void Update()
    {
        if (transform.position.y <= maxHeight)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //float _seconds = col.gameObject.CompareTag(playerTag) ? 0 : seconds; 
        if(col.gameObject.CompareTag(playerTag))
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 1);
            PNGUIController.Instance.setCoin();
            Destroy(gameObject);    
        }
        Destroy(gameObject, seconds);
    } 
}
