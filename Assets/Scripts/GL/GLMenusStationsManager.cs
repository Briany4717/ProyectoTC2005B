using Unity.VisualScripting;
using UnityEngine;


/// Gestiona la apertura y cierre de los menús para las diferentes estaciones de minijuegos.

public class GLMenusStationsManager : MonoBehaviour
{
    [SerializeField] private GameObject QuestionMenu;
    [SerializeField] private GameObject ControlMenu;
    [SerializeField] private GameObject RapidezMenu;

    [SerializeField] GLPlayerController playerController;
    [SerializeField] Interactor interactor;

    public static GLMenusStationsManager Instance { get; private set; }

    
    /// Configura el patrón Singleton para asegurar una única instancia del manejador.
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    
    /// Enumera los tipos de estaciones de minijuegos disponibles.
    
    public enum AvailableStations
    {
        Question,
        Control,
        Rapidez
    }

    
    /// Abre el menú correspondiente a la estación indicada y deshabilita los controles del jugador.
    
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

    
    /// Cierra todos los menús de estaciones y vuelve a habilitar los controles del jugador.
    
    public void CloseAllMenus()
    {
        QuestionMenu.SetActive(false);
        ControlMenu.SetActive(false);
        RapidezMenu.SetActive(false);
        playerController.enabled = true;
        interactor.enabled = true;
        GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.completeStation);
    }
}