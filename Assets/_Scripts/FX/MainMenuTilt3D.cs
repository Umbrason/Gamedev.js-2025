using UnityEngine;

public class MainMenuTilt3D : MonoBehaviour
{
    Spring.Config springConfig = new(20, .6f);
    RotationVector2Spring rotSpring;
    public float moveAmount = 0.5f;             // Sensitivity to mouse movement

    public float rotStrength = 10f;
    public float baseAspectRatio = 1.77f;       // Default is 16:9 (1.77:1)

    void Awake() => rotSpring = new(springConfig);

    void Update()
    {
        var mouseX = (Mathf.Clamp01(Input.mousePosition.x / Screen.width) - 0.5f) * 2f;
        var mouseY = (Mathf.Clamp01(Input.mousePosition.y / Screen.height) - 0.5f) * 2f;
        rotSpring.RestingPos = new(-mouseY * rotStrength, mouseX * rotStrength);
        rotSpring.Step(Time.deltaTime);
        transform.localRotation = Quaternion.Euler(rotSpring.Position._x0y());
        /* 
                // Invert offsets
                float offsetX = Mathf.Clamp(-mouseX * moveAmount, -moveThreshold.x, moveThreshold.x);
                float offsetZ = Mathf.Clamp(-mouseY * moveAmount, -moveThreshold.y, moveThreshold.y);

                Vector3 targetPosition = new Vector3(
                    initialPosition.x + offsetX,
                    initialPosition.y,
                    initialPosition.z + offsetZ
                );

                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * smoothSpeed); */
    }
}



