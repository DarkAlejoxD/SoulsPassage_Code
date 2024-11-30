using UnityEditor;

namespace UtilsComplements.Editor
{
    public class ScriptFromTemplate
    {
        private const string PATH_TO_BASETEMPLATE = "Assets/Scripts/Complements/Editor/BaseTemplate.cs.txt";
        private const string PATH_TO_CHEATTEMPLATE = "Assets/Scripts/Complements/Editor/CheatTemplate.cs.txt";

        [MenuItem(itemName: "Assets/Create/ScriptTemplates/C# UnityBase Template", isValidateFunction: false, priority = 1)]
        public static void CreateScriptFromTemplate_BaseTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(PATH_TO_BASETEMPLATE, "NewScript.cs");
        }

        [MenuItem(itemName: "Assets/Create/ScriptTemplates/C# Cheat Template", isValidateFunction: false, priority = 2)]
        public static void CreateScriptFromTemplate_CheatTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(PATH_TO_CHEATTEMPLATE, "Cheat_NewScript.cs");
        }
    }
}