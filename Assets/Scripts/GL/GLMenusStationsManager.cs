using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Gestiona la apertura y cierre de los menús para las diferentes estaciones de minijuegos.
/// </summary>
public class GLMenusStationsManager : MonoBehaviour
{
    [SerializeField] private GameObject QuestionMenu;
    [SerializeField] private GameObject ControlMenu;
    [SerializeField] private GameObject RapidezMenu;

    [SerializeField] GLPlayerController playerController;
    [SerializeField] Interactor interactor;

    public static GLMenusStationsManager Instance { get; private set; }

    /// <summary>
    /// Configura el patrón Singleton para asegurar una única instancia del manejador.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Enumera los tipos de estaciones de minijuegos disponibles.
    /// </summary>
    public enum AvailableStations
    {
        Question,
        Control,
        Rapidez
    }

    /// <summary>
    /// Abre el menú correspondiente a la estación indicada y deshabilita los controles del jugador.
    /// </summary>
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

    /// <summary>
    /// Cierra todos los menús de estaciones y vuelve a habilitar los controles del jugador.
    /// </summary>
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