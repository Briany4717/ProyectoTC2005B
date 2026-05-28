using UnityEngine;
using UnityEngine.EventSystems;


/// Permite interactuar con la representación en UI de los enemigos.

public class EnemigoUI : MonoBehaviour, IPointerClickHandler
{
    private Enemigo enemigoData;

    
    /// Asigna los datos del enemigo correspondientes a esta instancia de UI.
    
    public void Inicializar(Enemigo data)
    {
        enemigoData = data;
    }

    
    /// Maneja el clic sobre el enemigo para electrocutarlo si hay rayos disponibles.
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SistemaRayos.instancia.UsarRayo())
        {
            MusicController.instancia?.PlayElectricShock();
            SistemaMonedas.instancia?.RegistrarEliminarEnemigo();
            SistemaEnemigos.instancia.EliminarEnemigo(enemigoData);
            Destroy(gameObject);
        }
    }
}