using DataView;
using UnityEngine;

public class GhostBuildingView : EnumView<Building>
{
    [SerializeField] Material GhostMaterial;
    protected override void Refresh(Building? oldData, Building? newData)
    {
        base.Refresh(oldData, newData);
        if (!_instance) return;
        var srs = _instance.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in srs) sr.material = GhostMaterial;
    }
}
