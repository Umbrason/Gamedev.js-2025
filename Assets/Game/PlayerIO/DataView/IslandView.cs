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

        protected override void Refresh(PlayerIsland _, PlayerIsland newData)
        {
            var newTiles = newData.Tiles.ToDictionary(p => p.Key, p => !newData.Buildings.ContainsKey(p.Key) ? p.Value : Tile.None);
            var newBuildings = newData.Buildings;

            RefreshAtPositions<Tile, TileView>(newTiles, tileViewPrefab, _tileViews);
            RefreshAtPositions<Building, BuildingView>(newBuildings, buildingViewPrefab, _buildingViews);

            void RefreshAtPositions<T, TView>(IReadOnlyDictionary<HexPosition, T> newValues, TView prefab, Dictionary<HexPosition, TView> views)
                where TView : EnumView<T> where T : struct, System.Enum
            {
                // null-safe initialisierung (für den Fall, dass newValues intern null ist)
                HashSet<HexPosition> newPositions = newValues?.Keys.ToHashSet() ?? new();
                HashSet<HexPosition> oldPositions = views?.Keys.ToHashSet() ?? new();

                var deleted = oldPositions.Except(newPositions);
                var created = newPositions.Except(oldPositions);
                var updated = newPositions;

                foreach (var pos in deleted)
                {
                    if (views.TryGetValue(pos, out var view))
                    {
                        Destroy(view.gameObject);
                        views.Remove(pos);
                    }
                }

                foreach (var pos in created)
                {
                    var instance = Instantiate(prefab, (root ?? transform).position + pos.WorldPositionCenter, Quaternion.identity, root ?? transform);
                    views[pos] = instance;
                }

                foreach (var pos in updated)
                {
                    if (views.TryGetValue(pos, out var view) && newValues.TryGetValue(pos, out var value))
                    {
                        view.Data = value;
                    }
                }
            }
        }
    }
}