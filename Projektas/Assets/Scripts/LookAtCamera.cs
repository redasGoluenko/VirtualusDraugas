using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform playerCamera;  // Assign the player's camera here
    public float lookRange = 5f;    // Maximum distance at which the neck starts following the player
    public float minLookRange = 1f; // Minimum distance to avoid unnatural rotation when too close
    public float rotationSpeed = 5f; // Speed at which the neck rotates
    public float maxTurnAngle = 100f; // Maximum angle (in degrees) the neck can turn

    private Quaternion startRotation; // Store the initial rotation
    private Vector3 initialForwardDirection; // The initial forward direction the object is facing

    void Start()
    {
        startRotation = transform.rotation; // Save the starting rotation
        initialForwardDirection = transform.forward; // Save the initial forward direction
    }

    void Update()
    {
        if (playerCamera == null) return;

        // Calculate distance between neck and player camera
        float distance = Vector3.Distance(transform.position, playerCamera.position);

        if (distance <= lookRange && distance > minLookRange)
        {
            Vector3 direction = playerCamera.position - transform.position;
            direction.y = 0; // Ignore vertical movement for a natural look

            if (direction != Vector3.zero) // Prevent errors when normalizing zero vector
            {
                // Calculate the angle based on the initial forward direction of the object
                float angle = Vector3.Angle(initialForwardDirection, direction);

                // Debug log the angle relative to the initial forward direction
                Debug.Log("Angle relative to initial direction: " + angle);

                // Check if the camera is within the max turn angle in front of the neck
                if (angle <= maxTurnAngle)
                {
                    // Calculate the dot product to check if the camera is in front of the object
                    float dotProduct = Vector3.Dot(initialForwardDirection, direction.normalized);

                    // If the dot product is positive, the camera is in front of the object
                    if (dotProduct > 0)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                        return; // Stop further execution so it doesn't reset rotation
                    }
                }
            }
        }

        // If out of range, return to original rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, Time.deltaTime * rotationSpeed);
    }
}
