using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingPhaseSkip", menuName = "Scriptable Objects/AI/BuildingPhase/Skip")]
public class AIBuildPhaseSkipData : AIBuildPhaseData
{
    public override IEnumerator PlayingPhase(BuildPhase Phase, AIConfigData Config, GameInstance gameInstance)
    {
        yield return new WaitForSeconds(Config.ActionDelay);
        Phase.Skip();
    }
}
