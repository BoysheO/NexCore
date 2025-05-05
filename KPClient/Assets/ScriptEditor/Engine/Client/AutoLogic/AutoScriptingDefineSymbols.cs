using ScriptEditor.Engine.Client.Manager.ScriptingDefineSymbolsSystem;
using UnityEditor;

namespace SimEngineEditor
{
    internal static class AutoScriptingDefineSymbols
    {
        private const string CLIENT = "CLIENT";

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            ScriptingDefinesManager.Instance.Ensure(new[] { CLIENT });
        }
    }
}