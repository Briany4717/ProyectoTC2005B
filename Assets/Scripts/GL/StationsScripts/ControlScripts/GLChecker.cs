using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controla el movimiento de un objeto a lo largo de una ruta dibujada por el usuario.
/// </summary>
public class GLChecker : MonoBehaviour
{
    Vector2 startingPosition;
    private bool startMovement = false;
    Vector3[] positions;
    int movementIndex = 0;
    [SerializeField, Range(0.1f, 20f)] private float speed;
    [SerializeField] public DrawWithMouse drawControl;

    /// <summary>
    /// Guarda la posición inicial del objeto.
    /// </summary>
    private void Awake()
    {
        startingPosition = transform.position;
    }

    /// <summary>
    /// Inicia el trazo de la ruta cuando el jugador hace clic en el objeto.
    /// </summary>
    private void OnMouseDown()
    {
        drawControl.StartLine(this.transform.position);
        GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.SlideControlPuzzle);
    }

    /// <summary>
    /// Actualiza la línea dibujada mientras el usuario arrastra el mouse.
    /// </summary>
    private void OnMouseDrag()
    {
        drawControl.UpdateLine();
    }

    /// <summary>
    /// Comienza el movimiento a través de los puntos de la línea cuando se suelta el mouse.
    /// </summary>
    private void OnMouseUp()
    {
        positions = new Vector3[drawControl.line.positionCount];
        drawControl.line.GetPositions(positions);
        startMovement = true;
        movementIndex = 0;
    }

    /// <summary>
    /// Mueve el objeto hacia el siguiente punto de la ruta si el movimiento está activo.
    /// </summary>
    private void Update()
    {
        if (startMovement)
        {
            Vector2 currentPos = positions[movementIndex];
            transform.position = Vector2.MoveTowards(transform.position, currentPos, speed * Time.deltaTime);

            float distance = Vector2.Distance(currentPos, transform.position);
            if (distance <= 0.05f)
            {
                movementIndex++;
            }

            if (movementIndex > positions.Length - 1)
            {
                startMovement = false;
            }
        }
    }

    /// <summary>
    /// Detecta colisiones con los límites del juego y reinicia la posición.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GLDelimiter"))
        {
            RestartControlGame();
            GLSFXManager.Instance.PlaySFX(GLSFXManager.Instance.WallCrash);
        }
    }

    /// <summary>
    /// Restablece el juego de control a su estado inicial.
    /// </summary>
    public void RestartControlGame()
    {
        startMovement = false;
        movementIndex = 0;
        positions = null;

        Debug.Log("Tocaste Delimiter");
        transform.position = startingPosition;
        drawControl.ClearLine();
        drawControl.StartLine(startingPosition);
    }
}