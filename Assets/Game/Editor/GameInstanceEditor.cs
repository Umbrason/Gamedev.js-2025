#if UNITY_EDITOR
using UnityEngine;

[UnityEditor.CustomEditor(typeof(GameInstance))]
public class GameInstanceEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var phase = (target as GameInstance)?.CurrentPhase?.GetType()?.Name;
        GUILayout.Box(new GUIContent(phase), GUILayout.ExpandWidth(true));
    }
}
#endif