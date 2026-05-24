using UnityEngine.UI;

public class LEEmptyGraphic : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // Limpiamos los vértices por completo.
        // Esto hace que el objeto sea invisible y no genere ¡NI UN SOLO DRAW CALL NI OVERDRAW!
        vh.Clear(); 
    }
}