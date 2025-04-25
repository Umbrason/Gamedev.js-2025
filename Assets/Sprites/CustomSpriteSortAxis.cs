using UnityEngine;

public class CustomSpriteSortAxis : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    void Start()
    {
        Camera.transparencySortMode = TransparencySortMode.CustomAxis;
        Camera.transparencySortAxis = Camera.transform.forward;
    }
}
