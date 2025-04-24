using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ResourcesEstimator", menuName = "Scriptable Objects/ResourcesEstimator")]
public class AIResourcesEstimatorData : ScriptableObject
{
    [SerializeField, Min(0f)] private float basicResourcesBaseValue = 1f;
    [SerializeField, Min(0f)] private float combinedResourcesBaseValue = 3f;

    
    [SerializeField, Min(0f), Tooltip("How much is valued increasing the production by one")]
    private float basicResourcesBaseProductionValue = 10f;

    [SerializeField, Min(0f), Tooltip("How much is valued increasing the production by one")]
    private float combinedResourcesBaseProductionValue = 30f;

    public virtual ResourcesValues GetResourcesValues(GameInstance Game)
    {
        ResourcesValues values = new();

        foreach(Resource res in Enum.GetValues(typeof(Resource)))
        {
            bool basic = (int)res <= 6;

            values.Values[res] = basic ? basicResourcesBaseValue : combinedResourcesBaseValue;

            values.ProductionValues[res] = basic ? basicResourcesBaseProductionValue : combinedResourcesBaseProductionValue;
        }

        return values;
    }

    public class ResourcesValues
    {
        /// <summary>
        /// How much is valued having one unit of resource
        /// </summary>
        public Dictionary<Resource, float> Values = new();

        /// <summary>
        /// How much is valued increasing the production by one
        /// </summary>
        public Dictionary<Resource, float> ProductionValues = new();
    }
}
