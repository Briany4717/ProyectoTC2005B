using System.Collections;
using UnityEngine;

public class PNGameController : MonoBehaviour
{
    public PNGameController Instance; 
    public PNGUIController uiController;
    public PNSceneChanger sceneChanger;
  
    public void Awake()
    {
        StopAllCoroutines();
        PlayerPrefs.SetInt("Coins", 0);
        PlayerPrefs.SetInt("HuntedPrompts", 0);
        Instance = this; 
        Instance.SetReferences(); 
        DontDestroyOnLoad(this.gameObject);
        sceneChanger = FindAnyObjectByType<PNSceneChanger>();
    }

    void SetReferences()
    {
        if(uiController != null)
            uiController = FindAnyObjectByType<PNGUIController>();
    }

    public void AddCoin()
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 1);
        uiController.setCoin();
    }

    public void SpendCoins()
    {
        int newLives = PlayerPrefs.GetInt("Coins") - 1;
        PlayerPrefs.SetInt("Lives", newLives);
    }

    public void gameOver(bool win)
    {
        StopAllCoroutines();
        sceneChanger.change("PNFinalScene");
    }

    void Start()
    {
    }
}