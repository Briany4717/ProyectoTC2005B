using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawWithMouse : MonoBehaviour
{
    private LineRenderer line;
    private Vector3 previousPosition;

    [SerializeField] private float minDistance = 0.1f;
    [SerializeField, Range(0.1f, 2f)] private float width;


    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 1;
        previousPosition = transform.position;
        line.startWidth = line.endWidth = width;
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            // get the position of the mouse in world coordinates
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // Obtiene la distancia absoluta entre la cámara y el plano Z=0
            float distanceToZZero = Mathf.Abs(Camera.main.transform.position.z);

            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distanceToZZero));

            // si se supera la distancia minimia entonces dibuja
            if (Vector3.Distance(currentPosition, previousPosition) > minDistance)
            {
                // si es el primer punto porque no ha cambiado desde start.
                if (previousPosition == transform.position)
                {
                    line.SetPosition(0, currentPosition);
                }
                else
                {
                    // aumentamen en 1 la lista de puntos de la linea
                    line.positionCount++;
                    // agregamos la posicion actual en la lista de puntos de la linea
                    line.SetPosition(line.positionCount - 1, currentPosition);

                }
                // actualizamos la posicion actual como la anterior.
                previousPosition = currentPosition;
            }

        }
    }
}
