using UnityEngine;

public class RotateBoneToAxis : MonoBehaviour
{
    private Quaternion originalRotation;
    private float originalZRotation;
    public bool var = false;

    void Start()
    {       
        originalRotation = transform.rotation;
       
        originalZRotation = transform.rotation.eulerAngles.z;
    }

    private void Update()
    {
        if (var)
        {
            UpdateRotationToBone();
            var = false;
        }
    }

    public void UpdateRotationToBone()
    {
        // Preserve the original X and Y rotation and reset the Z rotation to the original value
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, originalZRotation);
    }

}
