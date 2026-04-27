using UnityEngine;

public class GLChecker : MonoBehaviour
{
    private bool startMovement = false;
    Vector3[] positions;
    int movementIndex = 0;
    [SerializeField, Range(0.1f, 20f)] private float speed;
    [SerializeField] public DrawWithMouse drawControl;
    private void OnMouseDown()
    {
        drawControl.StartLine(this.transform.position);
    }

    private void OnMouseDrag()
    {
        drawControl.UpdateLine();
    }

    private void OnMouseUp()
    {
        // positions of each point of the line renderer.
        positions = new Vector3[drawControl.line.positionCount];
        drawControl.line.GetPositions(positions);
        startMovement = true;
        movementIndex = 0;
    }

    private void Update()
    {
        if (startMovement)
        {
            Vector2 currentPos = positions[movementIndex];
            transform.position = Vector2.MoveTowards(transform.position, currentPos, speed * Time.deltaTime);

            // check if the position move is more than a value
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
}
