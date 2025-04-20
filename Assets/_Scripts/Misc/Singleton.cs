/**
 * Singleton.cs
 * Author: Luke Holland (http://lukeholland.me/)
 * slight changes by Ole (dontDestroyOnLoad)
 */

using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _instance;
    private static bool _quitting = false;

    public static T Instance
    {
        get
        {
            if (_instance == null && !_quitting)
            {
                _instance = GameObject.FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).ToString());
                    _instance = go.AddComponent<T>();

                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    [Header("Singleton")]
    [SerializeField] protected bool dontDestroyOnLoad = false;

    protected virtual void Awake()
    {
        Debug.Log($"I Exist in Scene {gameObject.scene.name} on GameObject {gameObject.name}!");
        if (_instance == null) _instance = gameObject.GetComponent<T>();
        else if (_instance.GetInstanceID() != GetInstanceID())
        {
            Destroy(gameObject);
            throw new System.Exception(string.Format("Instance of {0} already exists, removing {1}", GetType().FullName, ToString()));
        }

        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnApplicationQuit()
    {
        _quitting = true;
    }

}