using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PetitionPhaseUtility", menuName = "Scriptable Objects/AI/Petition/Utility")]
public class AIPetitionPhaseUtilityData : AIPetitionPhaseData
{
    [SerializeField] AIUtilityEstimator UtilityEstimator;

    public override IEnumerator PlayingPhase(PetitionPhase Phase, AIPlayer AI)
    {
        yield return new WaitForSeconds(AI.Config.ActionDelay);


        ResourcesUtilities ResourcesUtilities = UtilityEstimator.GetResourcesUtilities(AI.GameInstance);

        var BestBuilding = UtilityEstimator.GetBestBuilding(AI.GameInstance, ResourcesUtilities, onlyAffordable: false);

        Dictionary<PlayerID, Dictionary<Resource, int>> ResourcesSources = new();
        Dictionary<Resource, int> ResourcesNeeded = BestBuilding.Building.ConstructionCosts();
        Dictionary<Resource, int> ClientStock = new(AI.GameInstance.ClientPlayerData.Resources);

        //Client pays what they can
        ClientStock.Remove(ResourcesNeeded, out var PaidByClient);
        foreach(Resource res in PaidByClient.Keys.ToList())
        {
            if (PaidByClient[res] == 0) PaidByClient.Remove(res);
        }
        if(PaidByClient.Count > 0) ResourcesSources[AI.GameInstance.ClientID] = PaidByClient;

        //Guesstimate other players stock
        var otherPlayersIds = GetPlayerIDs(AI.GameInstance, exept: AI.GameInstance.ClientID);
        Dictionary<PlayerID, Dictionary<Resource, float>> StockGuess = new();
        const float PROD_TO_STOCK_RATIO = 0.5f;
        foreach (PlayerID player in otherPlayersIds)
        {
            StockGuess[player] = AI.GameInstance.PlayerData[player].Island.GetAverageProduction();
            foreach(Resource res in StockGuess[player].Keys.ToList())
            {
                StockGuess[player][res] *= PROD_TO_STOCK_RATIO;
            }
        }

        //Source from players with highest estimated stock (incrementally)
        foreach((Resource res, int quantity) in ResourcesNeeded)
        {
            for (int i = 0; i < quantity; i++)
            {
                PlayerID richest = PlayerID.None;
                float max = -10000f;
                int equalities = 1;
                foreach ((PlayerID player, Dictionary<Resource, float> stock) in StockGuess)
                {
                    float s = stock.GetValueOrDefault(res);
                    if(s < max) continue;
                    if (s == max)
                    {                        
                        equalities++;
                        //avoid picking first player everytimes (especialy when no one has the resource)
                        if (Random.value < 1f / equalities) continue;
                    }
                    else equalities = 1;

                    max = s;
                    richest = player;
                }
                StockGuess[richest][res] = StockGuess[richest].GetValueOrDefault(res) - 1f;
                if (!ResourcesSources.ContainsKey(richest)) ResourcesSources[richest] = new();
                ResourcesSources[richest][res] = ResourcesSources[richest].GetValueOrDefault(res) + 1;
            }
        }

        AI.Log(string.Format("petitions for a {0}", BestBuilding.Building));

        BuildingPetition petition = new BuildingPetition(AI.GameInstance.ClientID, BestBuilding.Position, BestBuilding.Building, ResourcesSources);

        Phase.SubmitPetition(petition);
    }
}
