using DataView;
using UnityEngine;
using UnityEngine.Events;

public class GhostBuildingView : EnumView<Building>
{
    [SerializeField] StringEvent OnYieldChange;
    [SerializeField] GameInstance Game;

    [SerializeField] Material GhostMaterial;
    protected override void Refresh(Building? oldData, Building? newData)
    {
        base.Refresh(oldData, newData);
        if (!_instance) return;
        var srs = _instance.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in srs) sr.material = GhostMaterial;
    }

    private void Update()
    {
        if(data == null) return;
        if(Game == null) return;

        float yield = BuildingExtensions.ExpectedYield((Building)data, Game.ClientPlayerData.Island, HexOrientation.Active * transform.position);
        OnYieldChange.Invoke(Mathf.RoundToInt(yield * 100f) + "%");
    }
}
