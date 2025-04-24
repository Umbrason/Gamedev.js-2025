using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingPhaseSkip", menuName = "Scriptable Objects/AI/BuildingPhase/Skip")]
public class AIBuildPhaseSkipData : AIBuildPhaseData
{
    public override IEnumerator PlayingPhase(BuildPhase Phase, AIPlayer AI)
    {
        yield return new WaitForSeconds(AI.Config.ActionDelay);
        Phase.Skip();
    }
}
