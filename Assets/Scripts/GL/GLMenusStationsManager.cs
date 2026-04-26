using Unity.VisualScripting;
using UnityEngine;

public class GLMenusStationsManager : MonoBehaviour
{

    [SerializeField] private GameObject QuestionMenu;
    [SerializeField] private GameObject ControlMenu;
    [SerializeField] private GameObject RapidezMenu;


    public static GLMenusStationsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public enum AvailableStations
    {
        Question,
        Control,
        Rapidez
    }


    public void OpenMenu(AvailableStations stationMenu)
    {
        CloseAllMenus();

        switch (stationMenu)
        {
            case AvailableStations.Question:
                QuestionMenu.SetActive(true);
                break;
            case AvailableStations.Control:
                ControlMenu.SetActive(true);
                break;
            case AvailableStations.Rapidez:
                RapidezMenu.SetActive(true);
                break;
        }
    }

    public void CloseAllMenus()
    {
        QuestionMenu.SetActive(false);
        ControlMenu.SetActive(false);
        RapidezMenu.SetActive(false);
    }



}
