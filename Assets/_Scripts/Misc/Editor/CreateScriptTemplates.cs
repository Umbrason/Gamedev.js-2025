using UnityEditor;

// not mine, modified slightly
// allows to easily create different kinds of scripts (i.e. interfaces, SOs, etc.)
public static class CreateScriptTemplates
{
    const string path = "Assets/_Scripts/Misc/Editor/Templates/";

    [MenuItem("Assets/Create/Code/MonoBehaviour", priority = 0)]
    public static void CreateMonoBehaviourMenuItem()
    {
        string templatePath = path + "MonoBehaviour.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewMonoBehaviour.cs");
    }

    [MenuItem("Assets/Create/Code/ScriptableObject", priority = 1)]
    public static void CreateScriptableObjectMenuItem()
    {
        string templatePath = path + "ScriptableObject.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewScriptableObject.cs");
    }

    [MenuItem("Assets/Create/Code/Enum", priority = 5)]
    public static void CreateEnumMenuItem()
    {
        string templatePath = path + "Enum.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewEnum.cs");
    }

    [MenuItem("Assets/Create/Code/Singleton", priority = 2)]
    public static void CreateSingletonMenuItem()
    {
        string templatePath = path + "Singleton.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewSingleton.cs");
    }

    [MenuItem("Assets/Create/Code/Interface", priority = 3)]
    public static void CreateInterfaceMenuItem()
    {
        string templatePath = path + "Interface.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewInterface.cs");
    }

    [MenuItem("Assets/Create/Code/SerializableClass", priority = 4)]
    public static void CreateSerializableClassMenuItem()
    {
        string templatePath = path + "SerializableClass.cs.txt";

        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewClass.cs");
    }
}
