using DataView;
using UnityEngine;

public abstract class EnumView<T> : View<T> where T : System.Enum
{
    protected GameObject _instance;

    protected override void Refresh(T oldData, T newData)
    {
        if (newData.Equals(oldData)) return;

        Destroy(_instance);
        _instance = null;

        GameObject prefab = PrefabsSettings.GetPrefabFor<T>(newData);

        if (prefab == null) return;

        _instance = Instantiate(prefab, transform, false);
    }
}
