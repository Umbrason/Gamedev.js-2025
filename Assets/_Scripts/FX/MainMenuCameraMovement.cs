using UnityEngine;

public class MainMenuCameraMovement : MonoBehaviour
{
    public float moveAmount = 0.5f;             // Sensitivity to mouse movement
    public float smoothSpeed = 3f;              // Smoothing factor
    public Vector2 moveThreshold = new Vector2(1f, 0.5f); // Max X and Z offset

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Invert offsets by multiplying by -1
        float offsetX = Mathf.Clamp(-mouseX * moveAmount, -moveThreshold.x, moveThreshold.x);
        float offsetZ = Mathf.Clamp(-mouseY * moveAmount, -moveThreshold.y, moveThreshold.y);

        Vector3 targetPosition = new Vector3(
            initialPosition.x + offsetX,
            initialPosition.y,         // Y stays fixed
            initialPosition.z + offsetZ
        );

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }
}


