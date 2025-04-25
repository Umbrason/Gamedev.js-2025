using UnityEngine;

public class MainMenuCameraMovement : MonoBehaviour
{
    public float moveAmount = 0.5f;             // Sensitivity to mouse movement
    public float smoothSpeed = 3f;              // Smoothing factor
    public Vector2 moveThreshold = new Vector2(1f, 0.5f); // Max X and Z offset

    public float baseAspectRatio = 1.77f;       // Default is 16:9 (1.77:1)
    public float aspectOffsetMultiplier = 1f;   // Controls how much the aspect ratio affects the camera X

    private Vector3 initialPosition;

    void Start()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        float yOffset = (currentAspect - baseAspectRatio) * aspectOffsetMultiplier;

        // Adjust initial position based on screen width
        initialPosition = transform.localPosition + new Vector3(0f, yOffset, 0f);
        transform.localPosition = initialPosition;
    }

    void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Invert offsets
        float offsetX = Mathf.Clamp(-mouseX * moveAmount, -moveThreshold.x, moveThreshold.x);
        float offsetZ = Mathf.Clamp(-mouseY * moveAmount, -moveThreshold.y, moveThreshold.y);

        Vector3 targetPosition = new Vector3(
            initialPosition.x + offsetX,
            initialPosition.y,
            initialPosition.z + offsetZ
        );

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }
}



