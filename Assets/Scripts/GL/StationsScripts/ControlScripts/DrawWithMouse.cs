using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Permite dibujar una línea interactiva siguiendo el movimiento del cursor.
/// </summary>
public class DrawWithMouse : MonoBehaviour
{
    public GameObject tutorialPointer;
    public LineRenderer line;
    private Vector3 previousPosition;

    [SerializeField] private float minDistance = 0.1f;
    [SerializeField, Range(0.1f, 2f)] private float width;

    /// <summary>
    /// Inicializa las propiedades de la línea y muestra el puntero del tutorial.
    /// </summary>
    private void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 1;
        previousPosition = transform.position;
        line.startWidth = line.endWidth = width;

        StartCoroutine(DelayedNudge());
        tutorialPointer.SetActive(true);
    }

    /// <summary>
    /// Configura el punto de inicio de la línea dibujada.
    /// </summary>
    public void StartLine(Vector2 position)
    {
        line.positionCount = 1;
        line.SetPosition(0, position);
        previousPosition = position;
    }

    /// <summary>
    /// Actualiza la línea agregando nuevos puntos mientras el usuario presiona el mouse.
    /// </summary>
    public void UpdateLine()
    {
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            tutorialPointer.SetActive(false);
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            float distanceToZZero = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distanceToZZero));

            if (Vector3.Distance(currentPosition, previousPosition) > minDistance)
            {
                if (previousPosition == transform.position)
                {
                    line.SetPosition(0, currentPosition);
                }
                else
                {
                    line.positionCount++;
                    line.SetPosition(line.positionCount - 1, currentPosition);
                }
                previousPosition = currentPosition;
            }
        }
    }

    /// <summary>
    /// Borra la línea actual reiniciando su conteo de puntos.
    /// </summary>
    public void ClearLine()
    {
        Debug.Log("borraste la linea");
        if (line == null) return;
        line.positionCount = 1;
        previousPosition = transform.position;
    }

    /// <summary>
    /// Realiza un pequeño movimiento inicial para evitar problemas visuales en la línea.
    /// </summary>
    private IEnumerator DelayedNudge()
    {
        yield return null;
        transform.position += new Vector3(0.001f, 0f, 0f);
    }
}