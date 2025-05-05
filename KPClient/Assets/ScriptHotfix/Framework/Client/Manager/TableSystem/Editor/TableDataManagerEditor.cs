using System.IO;
using System.Linq;
using BoysheO.Extensions;
using BoysheO.ProcessSystem;
using BoysheO.ProcessSystem.LogSystem;
using ScriptEngine.Editor.ClientCode.Manager.EnvironmentSystem;
using Sirenix.OdinInspector.Editor;
using UniRx;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Assets.HotScripts.Hotfix.GamePlay.FrameworkSystems.TableSystem.Editor
{
    public class TableDataManagerEditor : OdinEditorWindow
    {
        [MenuItem("Engine/TableSystem/OpenTableDir")]
        public static void OpenTableDir()
        {
            var config = EnvironmentHelper.GetEnvironment();
            var tableDir = config.Root.AsPath().Combine(config.Solution).Combine(@"Resources\Tables\Both").Value
                .ReplaceBackslash();
            EditorUtility.RevealInFinder(tableDir);
        }


        [MenuItem("Engine/TableSystem/GenTable")]
        public static async void GenTable()
        {
            var config = EnvironmentHelper.GetEnvironment();
            var tableGenPrj = config.Root.AsPath().Combine(config.Solution).Combine(@"RunTableGen").Value
                .ReplaceBackslash();

            EditorUtility.DisplayProgressBar("GenTable", "", 0);
            try
            {
                var subject = new Subject<Log>();
                subject.Subscribe(v =>
                {
                    if (v.Level == LogLevel.N)
                    {
                        Debug.Log(v.Text);
                    }
                    else if (v.Level == LogLevel.E)
                    {
                        Debug.LogError(v.Text);
                    }
                    else
                    {
                        Debug.Log($"{v.Level} {v.Text}");
                    }
                });
                var (isSuccesss, consoleLog, exitCode) =
                    await CommandLineHelper.ExecuteCommandAsync("dotnet", "run",
                        requireElevation: true, subject, workingDirectory: tableGenPrj);
                if (!isSuccesss || exitCode != 0)
                {
                    var totalLog = consoleLog.Select(v => $"{v.Level} {v.Text}").JoinAsOneString("\n");
                    var file = Path.GetTempFileName() + ".log";
                    await File.WriteAllTextAsync(file, totalLog);
                    EditorUtility.OpenWithDefaultApp(file);
                }

                AssetDatabase.Refresh();
                Debug.Log("GenTable done");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Debug/TableDataManager")]
        public static void OpenTableDataManagerWindow()
        {
            var win = GetWindow<TableDataManagerEditor>();
            win.Show();
        }

        // [Button]
        // void Load()
        // {
        //     var tbMgr = DIContext.ServiceProvider.GetRequiredService<ITableDataManager>();
        //     _configTables = tbMgr.Get<configTable>();
        //     _buildingTables = tbMgr.Get<buildingTable>();
        //     _powerFactory = tbMgr.GetRequire<int, buildingTable>(141).ResCapacityAsIns.ToDictionary(v=>v.Key.Id,v=>v.Value);
        // }
        //
        // [TableList] [ShowInInspector] private IReadOnlyList<configTable> _configTables;
        // [TableList] [ShowInInspector] private IReadOnlyList<buildingTable> _buildingTables;

        // [ShowInInspector]
        // private IReadOnlyDictionary<int,int> _powerFactory;
    }
}