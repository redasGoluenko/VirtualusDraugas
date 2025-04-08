using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> wayPoints = new List<Transform>();
    public int waypointIndex;
    public float moveSpeed;
    public float rotationSpeed;  // Sta�i� pasukim� greitis laipsniais per sekund�
    public bool isLoop;
    public bool isRandom;
    public bool isMoving;

    public Animator playerAnim;
    public Rigidbody rb; // Priskirk Rigidbody per Inspector arba per GetComponent

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        // U��aldome X ir Z a�i� rotacij�, kad fizika nesukelt� netik�t� pasukim�
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        StartMoving();
    }

    public void StartMoving()
    {
        playerAnim.SetBool("walk", true);
        Debug.Log("Walk parameter set to: " + playerAnim.GetBool("walk"));
        waypointIndex = 0;
        isMoving = true;
    }

    void FixedUpdate()
    {
        if (!isMoving)
            return;

        if (waypointIndex < wayPoints.Count)
        {
            // Tikslinio waypoint pozicija
            Vector3 targetPosition = wayPoints[waypointIndex].position;
            // Dabartin� pozicija (Rigidbody pozicija)
            Vector3 currentPosition = rb.position;

            // I�skiriame horizontalias koordinates, paliekame dabartin� y, kad gravitacija veikt�
            Vector3 horizontalCurrent = new Vector3(currentPosition.x, 0f, currentPosition.z);
            Vector3 horizontalTarget = new Vector3(targetPosition.x, 0f, targetPosition.z);

            // Apskai�iuojame krypt� � waypoint
            Vector3 direction = (horizontalTarget - horizontalCurrent).normalized;
            if (direction != Vector3.zero)
            {
                // Apskai�iuojame pageidaujam� rotacij�, kad veik�jas �i�r�t� � waypoint
                Quaternion desiredRotation = Quaternion.LookRotation(direction, Vector3.up);
                // Apribojame kiek laipsni� pasukimo gali �vykti per vien� FixedUpdate � sklandus per�jimas
                float maxRotationDegrees = rotationSpeed * Time.fixedDeltaTime;
                Quaternion smoothedRotation = Quaternion.RotateTowards(rb.rotation, desiredRotation, maxRotationDegrees);
                rb.MoveRotation(smoothedRotation);
            }

            // Judame veik�j� link waypoint, nekeisdami vertikalios (y) pad�ties
            Vector3 newPosition = Vector3.MoveTowards(
                                        currentPosition,
                                        new Vector3(targetPosition.x, currentPosition.y, targetPosition.z),
                                        Time.fixedDeltaTime * moveSpeed);
            rb.MovePosition(newPosition);

            // Tikriname, ar pasiek�me waypoint horizontaliai
            float distance = Vector3.Distance(horizontalCurrent, horizontalTarget);
            if (distance <= 0.05f)
            {
                if (isRandom)
                    waypointIndex = Random.Range(0, wayPoints.Count);
                else
                {
                    waypointIndex++;
                    if (isLoop && waypointIndex >= wayPoints.Count)
                        waypointIndex = 0;
                }
            }
        }
    }
}
