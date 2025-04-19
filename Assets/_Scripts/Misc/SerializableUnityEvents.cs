using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.Events
{
    [System.Serializable] public class FloatEvent : UnityEvent<float> { }
    [System.Serializable] public class IntEvent : UnityEvent<int> { }
    [System.Serializable] public class BoolEvent : UnityEvent<bool> { }
    [System.Serializable] public class StringEvent : UnityEvent<string> { }
}

