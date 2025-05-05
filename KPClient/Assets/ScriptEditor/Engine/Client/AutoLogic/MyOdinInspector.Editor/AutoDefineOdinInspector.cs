using System.IO;
using ScriptEditor.Engine.Client.Manager.ScriptingDefineSymbolsSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace SimEngineEditor
{
    public class AutoDefineOdinInspector
    {
        //如果OdinInspector已经导入，则添加OdinInspector的宏；如果OdinInspector缺失，则删除掉OdinInspector的宏定义
        //这个脚本是防止新手框架用户初始进入报错
        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            var sampler = CustomSampler.Create(nameof(AutoDefineOdinInspector));
            sampler.Begin();
            var odinDir = Application.dataPath + "/Plugins/Sirenix";
            var mgr = ScriptingDefinesManager.Instance;
            var defines = new[]
            {
                "ODIN_INSPECTOR",
                "ODIN_INSPECTOR_3",
                "ODIN_INSPECTOR_3_1",
                "ODIN_INSPECTOR_3_2",
                "ODIN_INSPECTOR_3_3",
            };
            if (!Directory.Exists(odinDir))
            {
                if (mgr.Remove(defines))
                {
                    Debug.Log($"[{nameof(AutoDefineOdinInspector)}]Remove defines");
                }
            }
            else
            {
                if (mgr.Ensure(defines))
                {
                    Debug.Log($"[{nameof(AutoDefineOdinInspector)}]Add defines");
                }
            }
            sampler.End();
        }
    }
}