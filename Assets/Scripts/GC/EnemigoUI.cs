using UnityEngine;
using UnityEngine.EventSystems;

public class EnemigoUI : MonoBehaviour, IPointerClickHandler
{
    private Enemigo enemigoData;

    public void Inicializar(Enemigo data)
    {
        enemigoData = data;
    }

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