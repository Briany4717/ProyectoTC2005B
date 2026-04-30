using Unity.VisualScripting;
using UnityEngine;

public class GLMenusStationsManager : MonoBehaviour
{

    [SerializeField] private GameObject QuestionMenu;
    [SerializeField] private GameObject ControlMenu;
    [SerializeField] private GameObject RapidezMenu;


    // obtenemos los scripts que queremos desactivar
    [SerializeField] GLPlayerController playerController;
    [SerializeField] Interactor interactor;



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
        playerController.enabled = false;
        interactor.enabled = false;
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
        playerController.enabled = true;
        interactor.enabled = true;
    }




}
