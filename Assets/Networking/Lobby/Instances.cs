using UnityEngine;

public class Instances : Singleton<Instances>
{
    [SerializeField]
    private GameInstance[] allGameInstances;

    public GameInstance[] AllGameInstances { get => allGameInstances; }
}