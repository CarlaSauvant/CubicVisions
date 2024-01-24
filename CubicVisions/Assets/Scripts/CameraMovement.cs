using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    // Public variables for customization in the Inspector
    public Transform selectionGrid;
    public float moveSpeed = 100f;
    public Vector3 positionOffset = new Vector3(0f, 10f, 40f);
    public Vector3 rotationAngle = new Vector3(10f, 180f, 0f);
    public Vector3 initialCameraPosition = new Vector3(0f, 380.3f, 384.4f);
    public Quaternion initialCameraRotation = Quaternion.Euler(45f, 180f, 0f);

    void Start()
    {
        // Assuming you have a UI button attached to the script
        Button enterButton = GetComponent<Button>();

        // Attach a method to the button click event
        if (enterButton != null)
        {
            enterButton.onClick.AddListener(MoveCameraToSelectionGrid);
        }
    }

    public void MoveCameraToSelectionGrid()
    {
        if (selectionGrid != null)
        {
            // Calculate the target position based on the selection grid and offset
            targetPosition = selectionGrid.position + positionOffset;
            initialPosition = transform.position;

            // Set the initial and target rotations based on the specified angles
            initialRotation = transform.rotation;
            targetRotation = Quaternion.Euler(rotationAngle);

            // Move the camera towards the target position
            StartCoroutine(MoveCameraCoroutine());
        }
    }

    System.Collections.IEnumerator MoveCameraCoroutine()
    {
        float totalDistance = Vector3.Distance(initialPosition, targetPosition);
        float moveDuration = totalDistance / moveSpeed;
        float rotationDuration = moveDuration;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // Incrementally move the camera towards the target position
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / moveDuration);

            // Incrementally interpolate the rotation towards the target rotation
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / rotationDuration);

            // Increase elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final positions and rotations are exactly as intended
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    public void MoveCameraRight()
    {
        if (selectionGrid != null)
        {
            // Start a coroutine to smoothly rotate the camera around the selectionGrid
            StartCoroutine(RotateCameraAroundGridCoroutine(90f)); // Rotate by 90 degrees for anti-clockwise rotation
        }
    }

    System.Collections.IEnumerator RotateCameraAroundGridCoroutine(float targetAngleChange)
    {
        float rotationDuration = 1f; // You can adjust the rotation speed

        Quaternion initialRotation = transform.rotation;
        Vector3 rotationCenter = selectionGrid.position; // Center of rotation
        float radius = Vector3.Distance(transform.position, rotationCenter);

        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            // Incrementally interpolate the rotation towards the target rotation
            Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, targetAngleChange, 0f);
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / rotationDuration);

            // Move the camera in a circular path around the selectionGrid
            float angle = Mathf.Lerp(0f, targetAngleChange, elapsedTime / rotationDuration);
            float x = rotationCenter.x + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = rotationCenter.z + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            transform.position = new Vector3(x, transform.position.y, z);

            // Make the camera look at the center of the selectionGrid
            transform.LookAt(rotationCenter);

            // Increase elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the final rotation is exactly as intended
        transform.rotation = initialRotation * Quaternion.Euler(0f, targetAngleChange, 0f);
        transform.LookAt(rotationCenter);
    }

    public void MoveCameraToStart()
    {
        transform.position = initialCameraPosition;
        transform.rotation = initialCameraRotation;
    }
}