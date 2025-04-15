using UnityEditor;
using UnityEngine;


// Most code and idea taken from https://youtu.be/za_pBB80Nt8
public class Developer
{
    //* -----------------------GAME----------------------------
    [MenuItem("Developer/Game/Clear Saves")]
    public static void ClearSaves()
    {
        // Maybe ask for confirmation?

        PlayerPrefs.DeleteAll();
        // Clear serialized Saves

    }


    [MenuItem("Developer/Game/Cheats/+Resources")]
    public static void AddResource()
    {
        // Give player 1 of each resource

    }


    //* -----------------------EDITOR----------------------------
    // taken from https://youtu.be/iAEh7FkY7o4
    [MenuItem("Developer/Editor/Missing Scripts/Find all")]
    public static void FindMissingScriptsMenuItem()
    {
        foreach (GameObject go in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            foreach (Component component in go.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    Debug.Log("GameObject found with missing script " + go.name);
                    break;
                }
            }
        }
    }

    // taken from https://youtu.be/iAEh7FkY7o4
    [MenuItem("Developer/Editor/Missing Scripts/Delete all")]
    public static void DeleteMissingScriptsMenuItem()
    {
        foreach (GameObject go in GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            foreach (Component component in go.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    break;
                }
            }
        }
    }
}