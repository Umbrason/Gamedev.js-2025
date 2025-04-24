using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResourcesQuantities))]
public class ResourcesQuantitiesDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Draw foldout with label
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            label,
            true
        );

        if (!property.isExpanded)
            return;

        EditorGUI.indentLevel++;

        SerializedProperty itemsProp = property.FindPropertyRelative("items");
        float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        for (int i = 0; i < itemsProp.arraySize; i++)
        {
            SerializedProperty element = itemsProp.GetArrayElementAtIndex(i);
            SerializedProperty resProp = element.FindPropertyRelative("res");
            SerializedProperty quantityProp = element.FindPropertyRelative("quantity");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float width = position.width;

            float removeButtonWidth = 20f;

            Rect resRect = new Rect(position.x, y, width * 0.5f, lineHeight);
            Rect quantityRect = new Rect(position.x + width * 0.55f, y, width * 0.35f - removeButtonWidth, lineHeight);
            Rect removeRect = new Rect(position.x + width - removeButtonWidth, y, removeButtonWidth, lineHeight);

            EditorGUI.PropertyField(resRect, resProp, GUIContent.none);
            EditorGUI.PropertyField(quantityRect, quantityProp, GUIContent.none);

            if (GUI.Button(removeRect, "-"))
            {
                itemsProp.DeleteArrayElementAtIndex(i);
                break; // Avoid layout issues after modifying the array
            }

            y += lineHeight + spacing;
        }

        // Add button
        Rect addBtn = new Rect(position.x, y, 40, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(addBtn, "+"))
        {
            itemsProp.InsertArrayElementAtIndex(itemsProp.arraySize);
        }

        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;

        SerializedProperty itemsProp = property.FindPropertyRelative("items");
        int count = itemsProp.arraySize;

        return EditorGUIUtility.singleLineHeight + // foldout
               (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * count + // elements
               EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // + button
    }
}
