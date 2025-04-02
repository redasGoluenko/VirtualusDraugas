using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform playerCamera;  // Assign the player's camera here
    public float lookRange = 5f;    // Distance at which the neck starts following the player
    public float rotationSpeed = 5f; // Speed at which the neck rotates

    void Update()
    {
        if (playerCamera == null) return;

        // Calculate distance between neck and player camera
        float distance = Vector3.Distance(transform.position, playerCamera.position);

        // If within range, rotate to face the camera
        if (distance <= lookRange)
        {
            Vector3 direction = playerCamera.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
