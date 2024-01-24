using UnityEngine;

public class MoveSelectionGrid : MonoBehaviour
{
    public float moveIncrement = 3.40201f;
    public float continuousMoveThreshold = 0.5f; // Time threshold for continuous movement
    public float boxWidth = 34f;
    public float boxHeight = 1f;
    public float boxLength = 34f;

    private float continuousMoveTimer = 0f;

    public DisappearButton disappearButton;

    void Update()
    {
        // Check if the button is active in the DisappearButton script
        if (disappearButton != null && disappearButton.IsButtonActive())
        {          
            // Check for individual arrow key presses
            if ((Input.GetKeyDown(KeyCode.LeftArrow) && continuousMoveTimer == 0f) ||
                (Input.GetKey(KeyCode.LeftArrow) && continuousMoveTimer > continuousMoveThreshold))
            {
                MoveWithCollisionDetection(Vector3.right);
                continuousMoveTimer = 0f; // Reset the timer when a key is pressed
            }
            else if ((Input.GetKeyDown(KeyCode.RightArrow) && continuousMoveTimer == 0f) ||
                (Input.GetKey(KeyCode.RightArrow) && continuousMoveTimer > continuousMoveThreshold))
            {
                MoveWithCollisionDetection(Vector3.left);
                continuousMoveTimer = 0f;
            }
            else if ((Input.GetKeyDown(KeyCode.UpArrow) && continuousMoveTimer == 0f) ||
                (Input.GetKey(KeyCode.UpArrow) && continuousMoveTimer > continuousMoveThreshold))
            {
                MoveWithCollisionDetection(Vector3.back);
                continuousMoveTimer = 0f;
            }
            else if ((Input.GetKeyDown(KeyCode.DownArrow) && continuousMoveTimer == 0f) ||
                (Input.GetKey(KeyCode.DownArrow) && continuousMoveTimer > continuousMoveThreshold))
            {
                MoveWithCollisionDetection(Vector3.forward);
                continuousMoveTimer = 0f;
            }

            // Increment the continuousMoveTimer when a key is held down
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
                Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                continuousMoveTimer += Time.deltaTime;
            }
            else
            {
                continuousMoveTimer = 0f; // Reset the timer when the key is released
            }
        }

        else
        {
            
        } 
    }

    void MoveWithCollisionDetection(Vector3 movement)
    {
        // Calculate the target position
        Vector3 targetPosition = transform.position + RoundVector(movement) * moveIncrement;

        // Perform a simple collision check using a box cast
        if (!CheckCollision(targetPosition, movement.normalized))
        {
            // Move the object if there's no collision
            transform.position = targetPosition;
        }
    }

    bool CheckCollision(Vector3 targetPosition, Vector3 direction)
    {
        // Set the size of the box based on the parameters
        Vector3 boxSize = new Vector3(boxWidth, boxHeight, boxLength);

        // Perform a box cast from the current position to the target position
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, boxSize / 2, direction, out hit, Quaternion.identity, moveIncrement))
        {
            /// If the box cast hits something, there's a collision
            // Debug.Log("Collision detected with " + hit.collider.gameObject.name);
            return true;
        }

        // No collision
        return false;
    }

    Vector3 RoundVector(Vector3 input)
    {
        // Round each component of the vector to the nearest whole number
        return new Vector3(Mathf.Round(input.x), Mathf.Round(input.y), Mathf.Round(input.z));
    }
}