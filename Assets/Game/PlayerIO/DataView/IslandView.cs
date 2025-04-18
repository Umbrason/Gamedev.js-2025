using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataView
{
    public class IslandView : View<PlayerIsland>
    {
        [SerializeField] TileView tileViewPrefab;
        [SerializeField] BuildingView buildingViewPrefab;
        [SerializeField] Transform root;

        private Dictionary<HexPosition, TileView> _tileViews = new();
        private Dictionary<HexPosition, BuildingView> _buildingViews = new();
        protected override void Refresh(PlayerIsland oldData, PlayerIsland newData)
        {
            RefreshAtPositions<Tile, TileView>(newData.Tiles, tileViewPrefab, _tileViews);
            RefreshAtPositions<Building, BuildingView>(newData.Buildings, buildingViewPrefab, _buildingViews);

            void RefreshAtPositions<T, TView>(IReadOnlyDictionary<HexPosition, T> newValues, TView prefab, Dictionary<HexPosition, TView> views)
                where TView : View<T>
            {
                HashSet<HexPosition> newPositions = newValues?.Keys.ToHashSet();
                HashSet<HexPosition> oldPositions = views.Keys?.ToHashSet();

                var deleted = oldPositions?.Except(newPositions);
                var created = newPositions?.Except(oldPositions);
                var updated = newPositions?.Union(newPositions);

                foreach (var pos in deleted)
                {
                    Destroy(_tileViews[pos].gameObject);
                    views.Remove(pos);
                }

                foreach (var pos in created)
                {
                    views[pos] = Instantiate(prefab, (root ?? transform).position + pos.WorldPositionCenter, Quaternion.identity, root ?? transform);
                }

                foreach (var pos in newPositions)
                {
                    views[pos].Data = newValues[pos];
                }
            }
        }
    }
}
