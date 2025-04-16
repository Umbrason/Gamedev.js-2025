using UnityEditor;
using UnityEngine;
using DataView;
using System;

[CustomEditor(typeof(BuildingsPrefabsData))]
public class BuildingsPrefabsDataEditor : Editor
{
    private SerializedProperty buildingsPrefabsProps;
    private string[] buildingsNames;
    private int enumLength;

    private void OnEnable()
    {
        buildingsPrefabsProps = serializedObject.FindProperty("buildingsPrefabs");

        Type enumType = typeof(Building);
        buildingsNames = Enum.GetNames(enumType);
        enumLength = buildingsNames.Length;

        if (buildingsPrefabsProps.arraySize != enumLength)
        {
            buildingsPrefabsProps.arraySize = enumLength;
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Buildings Prefabs", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        for (int i = 0; i < enumLength; i++)
        {
            SerializedProperty element = buildingsPrefabsProps.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(element, new GUIContent(buildingsNames[i]));
        }

        EditorGUI.indentLevel--;
        serializedObject.ApplyModifiedProperties();
    }
}
