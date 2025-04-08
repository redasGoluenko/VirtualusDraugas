using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform playerCamera;  // Assign the player's camera here
    public float lookRange = 3f;    // Maximum distance at which the neck starts following the player
    public float minLookRange = 0.1f; // Minimum distance to avoid unnatural rotation when too close
    public float rotationSpeed = 5f; // Speed at which the neck rotates
    public float maxTurnAngle = 100f; // Maximum angle (in degrees) the neck can turn

    private Quaternion startRotation; // Store the initial rotation
    private Vector3 initialForwardDirection; // The initial forward direction the object is facing
    private Vector3 forward; // The forward direction of the object

    public WaypointManager waypointManager; // Reference to the WaypointManager script
    private bool flag = false;
    private bool flag2 = false;

    void Start()
    {
        startRotation = transform.rotation; // Save the starting rotation
        initialForwardDirection = transform.forward; // Save the initial forward direction
    }

    void LateUpdate() // <- Moved from Update to LateUpdate
    {
        if (playerCamera == null) return;

        float distance = Vector3.Distance(transform.position, playerCamera.position);

        // If the camera is in range and the object is not already tracking, start tracking
        if (distance <= lookRange && distance > minLookRange)
        {
            flag2 = true;
            if (!flag)
            {
                forward = transform.forward;
                Debug.Log("LookAtCamera: Camera entered range");
                flag = true;
                
            }

            Vector3 direction = playerCamera.position - transform.position;
            direction.y = 0; // Keep it level

            // Only proceed if the direction is valid (not zero)
            if (direction != Vector3.zero)
            {
                // Calculate the angle between the object's current forward and the camera's direction
                float angle = Vector3.Angle(forward, direction);  // Use current forward direction

                // Log the angle for debugging purposes
                Debug.Log($"Angle to camera: {angle} degrees");

                // If the angle is within the max allowed rotation, check if the camera is in front
                if (angle <= maxTurnAngle)
                {
                    waypointManager.StopMoving(); // Stop the movement when camera is in range
                    float dotProduct = Vector3.Dot(forward, direction.normalized);

                    // Check if the dot product is positive, meaning the camera is in front
                    if (dotProduct > 0)
                    {
                        // Rotate smoothly towards the camera's position
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                    }
                }
            }
        }
        else
        {
            flag2 = false;
            if (flag)
            {
                // Camera has exited the range, reset flag and resume movement
                Debug.Log("LookAtCamera: Camera exited range");
                waypointManager.StartMoving(); // Resume movement
                flag = false; // Reset the flag
            }

            // Only return to the initial rotation if the object isn't facing the camera
            if (Vector3.Angle(initialForwardDirection, transform.forward) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * rotationSpeed);
            }
        }

        // Update the initial forward direction only when it starts tracking the camera
        if (flag && flag2)
        {
            initialForwardDirection = transform.forward;  // Keep this updated continuously
        }
    }
}
