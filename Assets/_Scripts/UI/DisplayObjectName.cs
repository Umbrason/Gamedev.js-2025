using UnityEngine;
using UnityEngine.Events;

public class DisplayObjectName : MonoBehaviour
{
    [SerializeField] StringEvent ObjectName;
    void Start()
    {
        ObjectName?.Invoke(name.Replace("(Clone)",""));
    }
}
