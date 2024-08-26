using UnityEngine;

public class freeCamera : MonoBehaviour
{
    public Transform target; 
    private float rotationSpeed = 50f; 
    private float radius = 1000f; 
    private float verticalOffset = 400f;
    private float currentAngle = 0f;

    void Start()
    {
        UpdateCameraPosition();
    }

    void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); 
        currentAngle += horizontalInput * rotationSpeed * Time.deltaTime;
        currentAngle %= 360;
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        
        float radianAngle = currentAngle * Mathf.Deg2Rad;
        float x = target.position.x + Mathf.Cos(radianAngle) * radius;
        float z = target.position.z + Mathf.Sin(radianAngle) * radius;
        transform.position = new Vector3(x, target.position.y + verticalOffset, z);
        transform.LookAt(target.position);
    }
}