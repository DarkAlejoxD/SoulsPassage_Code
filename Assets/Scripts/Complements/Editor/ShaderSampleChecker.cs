using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UtilsComplements.Editor
{
    [ExecuteInEditMode]
    public class ShaderSampleChecker : EditorWindow
    {
        private const string ShaderPattern = @"sampler2D";
        private const string ShaderGraphPattern = @"""m_Samples""";
        private const int SampleLimit = 16; // Define tu límite aquí

        [MenuItem("Tools/Check Shader Samples")]
        public static void ShowWindow()
        {
            GetWindow<ShaderSampleChecker>("Check Shader Samples");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Check Shaders"))
            {
                CheckShaders();
            }
        }

        private void CheckShaders()
        {            
            string[] shaderFiles = Directory.GetFiles(Application.dataPath, "*.shader", SearchOption.AllDirectories);
            string[] shaderGraphFiles = Directory.GetFiles(Application.dataPath, "*.shadergraph", SearchOption.AllDirectories);

            foreach (string shaderFile in shaderFiles)
            {
                CheckShaderFile(shaderFile);
            }

            foreach (string shaderGraphFile in shaderGraphFiles)
            {
                CheckShaderGraphFile(shaderGraphFile);
            }

            Debug.Log("Shader sample check completed.");
        }

        private void CheckShaderFile(string shaderFile)
        {
            string content = File.ReadAllText(shaderFile);
            int sampleCount = Regex.Matches(content, ShaderPattern).Count;

            if (sampleCount > SampleLimit)
            {
                Debug.LogWarning($"Shader '{shaderFile}' exceeds sample limit with {sampleCount} samples.");
            }
        }

        private void CheckShaderGraphFile(string shaderGraphFile)
        {
            string content = File.ReadAllText(shaderGraphFile);
            int sampleCount = Regex.Matches(content, ShaderGraphPattern).Count;

            if (sampleCount > SampleLimit)
            {
                Debug.LogWarning($"ShaderGraph '{shaderGraphFile}' exceeds sample limit with {sampleCount} samples.");
            }
        }
    }
}
