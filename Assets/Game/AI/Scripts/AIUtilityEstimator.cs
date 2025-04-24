using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ResourcesUtilityEstimator", menuName = "Scriptable Objects/ResourcesUtilityEstimator")]
public partial class AIUtilityEstimator : ScriptableObject
{
    [SerializeField, Min(0f)] private float basicResourcesBaseValue = 1f;
    [SerializeField, Min(0f)] private float combinedResourcesBaseValue = 3f;

    
    [SerializeField, Min(0f), Tooltip("How much is valued increasing the production by one")]
    private float basicResourcesBaseUtility = 10f;

    [SerializeField, Min(0f), Tooltip("How much is valued increasing the production by one")]
    private float combinedResourcesBaseUtility = 30f;

    public virtual ResourcesUtilities GetResourcesUtilities(GameInstance Game)
    {
        ResourcesUtilities values = new();

        foreach(Resource res in Enum.GetValues(typeof(Resource)))
        {
            bool basic = (int)res <= 6;

            values.Stock[res] = basic ? basicResourcesBaseValue : combinedResourcesBaseValue;

            values.Prodution[res] = basic ? basicResourcesBaseUtility : combinedResourcesBaseUtility;
        }

        return values;
    }
}
