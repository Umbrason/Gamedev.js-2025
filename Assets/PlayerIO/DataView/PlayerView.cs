using UnityEngine;

namespace DataView
{
    public class PlayerView : View<PlayerData>
    {
        [SerializeField] private IslandView islandView;
        protected override void Refresh(PlayerData oldData, PlayerData newData)
        {
            islandView.Data = newData.Island;
        }
    }
}
