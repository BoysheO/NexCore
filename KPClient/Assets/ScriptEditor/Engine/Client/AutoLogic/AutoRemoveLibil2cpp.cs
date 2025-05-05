using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class AutoRemoveLibil2cpp
{
#if UNITY_IOS
    
    [PostProcessBuild]
    private static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
        {
            //Debug.Log("Not an iOS build. Skipping libil2cpp.a deletion.");
            return;
        }

        string libPath = Path.Combine(pathToBuiltProject, "Libraries/libil2cpp.a");

        // 1. 删除文件
        if (File.Exists(libPath))
        {
            File.Delete(libPath);
            Debug.Log($"Deleted file: {libPath}");
        }
        else
        {
            Debug.Log($"File not found, skipping deletion: {libPath}");
        }

        // 2. 修改 Xcode 工程，移除引用
        string pbxProjectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject project = new PBXProject();
        project.ReadFromFile(pbxProjectPath);

        string targetGuid = project.GetUnityMainTargetGuid();
        string fileGuid = project.FindFileGuidByProjectPath("Libraries/libil2cpp.a");

        if (!string.IsNullOrEmpty(fileGuid))
        {
            project.RemoveFile(fileGuid);
            project.RemoveFileFromBuild(targetGuid, fileGuid);
            Debug.Log("Removed libil2cpp.a reference from Xcode project.");
        }
        else
        {
            Debug.Log("libil2cpp.a not found in PBXProject. Skipping Xcode reference removal.");
        }

        project.WriteToFile(pbxProjectPath);
    }
#endif
}