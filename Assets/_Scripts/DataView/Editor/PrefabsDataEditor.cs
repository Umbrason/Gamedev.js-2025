using UnityEditor;
using UnityEngine;
using DataView;
using System;

[CustomEditor(typeof(PrefabsSettings))]
public class PrefabsDataEditor : Editor
{
    private EnumArrayProp<Tile, GameObject> tilesProp;
    private EnumArrayProp<Building, GameObject> buildingsProp;

    private void OnEnable()
    {
        tilesProp = new("tilesPrefabs", serializedObject, "Tiles Prefabs");
        buildingsProp = new("buildingsPrefabs", serializedObject, "Buildings Prefabs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        tilesProp.DrawField();
        EditorGUILayout.Space();
        buildingsProp.DrawField();

        serializedObject.ApplyModifiedProperties();
    }

    class EnumArrayProp<TEnum, TElements>
    {
        SerializedProperty property;
        private string[] names;
        private int length;
        private string label;

        public EnumArrayProp(string propertyName, SerializedObject serializedObject, string label)
        {
            this.label = label;
            property = serializedObject.FindProperty(propertyName);

            Type enumType = typeof(TEnum);
            names = Enum.GetNames(enumType);
            length = names.Length;

            if (property.arraySize != length)
            {
                property.arraySize = length;
                serializedObject.ApplyModifiedProperties();
            }
        }

        public void DrawField()
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            for (int i = 0; i < length; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element, new GUIContent(names[i]));
            }

            EditorGUI.indentLevel--;
        }
    }
}
