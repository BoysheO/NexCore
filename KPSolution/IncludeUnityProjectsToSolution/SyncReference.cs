using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SolutionConfigSpace;

namespace WorkFlow.Command;

public class SyncReference
{
    public async Task ExecuteAsync()
    {
        await AddCsprojToSln();
        await ReferenceFix();
    }

    /// <summary>
    /// 将U3d工程下的csproj同步到本工程Mono解决方案文件夹下
    /// </summary>
    /// <exception cref="Exception"></exception>
    /// <exception cref="NotImplementedException"></exception>
    private async Task AddCsprojToSln()
    {
        var config = new SolutionConfig();
        var sln = config.SlnFile;
        var (isSuccess, log, code) = await ProcessHelper.InvokeAsync("dotnet", $"sln {sln.FullName} list");
        if (!isSuccess) throw new Exception("dotnet sln list failed");

        #region 清除老的csproj 可能清理不干净，需要手动清理

        var regex = @"^\.\.\\{Unity3DProjectName}\\[a-zA-Z0-9\.-]+\.csproj".Replace("{Unity3DProjectName}",
            config.Unity3DProjectName);
        var matches = Regex.Matches(log, regex, RegexOptions.Multiline);
        foreach (Match match in matches)
        {
            var pjPath = match.Value.Replace("..", sln.Directory.Parent.FullName);
            await ProcessHelper.InvokeAsync("dotnet", $"sln {sln.FullName} remove {pjPath}");
        }

        #endregion

        #region 添加新的csproj

        var u3dPrj = config.Unity3DProjectDir;
        var csprj = u3dPrj.GetFiles("*.csproj");

        // dotnet sln to_do.sln add to_do-app/to_do-app.csproj
        foreach (var info in csprj)
        {
            await ProcessHelper.InvokeAsync("dotnet", $"sln {sln.FullName} add {info.FullName} --solution-folder Mono");
        }

        #endregion
    }

    /// <summary>
    /// 将Assembly-CSharp的引用同步到Hotfix工程 *无法区分Editor的dll，所以部分需要手动设置
    /// 将HotfixMono目录下的所有工程引用添加到Hotfix工程
    /// </summary>
    private async Task ReferenceFix()
    {
        //todo 
    }
}