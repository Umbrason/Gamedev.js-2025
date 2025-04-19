using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(EnumSpriteLib<>))]
public class EnumSpriteLibEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var targetType = target.GetType();
        var enumType = targetType.BaseType.GetGenericArguments()[0];
        var keysProperty = serializedObject.FindProperty("keys");
        var valuesProperty = serializedObject.FindProperty("values");
        var enumValues = Enum.GetValues(enumType);
        keysProperty.arraySize = enumValues.Length;
        valuesProperty.arraySize = enumValues.Length;
        for (int i = 0; i < enumValues.Length; i++)
        {
            keysProperty.GetArrayElementAtIndex(i).SetEnumValue((Enum)enumValues.GetValue(i));
            EditorGUILayout.PropertyField(valuesProperty.GetArrayElementAtIndex(i), new GUIContent(enumValues.GetValue(i).ToString()));
        }
        serializedObject.ApplyModifiedProperties();
    }
}