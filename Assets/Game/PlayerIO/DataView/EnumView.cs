using System;
using DataView;
using UnityEngine;

public abstract class EnumView<T> : View<T?> where T : struct, System.Enum
{
    protected GameObject _instance;

    protected override void Refresh(T? oldData, T? newData)
    {
        if (newData.Equals(oldData)) return;

        Destroy(_instance);
        _instance = null;
        if (!newData.HasValue) return;
        GameObject prefab = PrefabsSettings.GetPrefabFor<T>(newData.Value);

        if (prefab == null) return;

        _instance = Instantiate(prefab, transform, false);
    }
}
