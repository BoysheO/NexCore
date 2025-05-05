using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BoysheO.Util;
using SolutionConfigSpace;

namespace ProgramConstConfigSystem
{
    public static class ProgramConstConfigGenerator
    {
        public static void Gen(Action<ProgramConstConfigModel>? modifyAction)
        {
            var config = new SolutionConfig();
            var json = File.ReadAllText(config.ProgramConstConfigJsonPath);
            var model = JsonSerializer.Deserialize<ProgramConstConfigModel>(json);
            modifyAction?.Invoke(model);
            json = JsonSerializer.Serialize(model);
            if (modifyAction != null) System.IO.File.WriteAllText(config.ProgramConstConfigJsonPath, json);
            
            #region 根据json内容生成代码.(为什么不是根据model呢？因为以下为祖传代码，能用）

            {
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var dic = new Dictionary<string, (string type, string value)>();
                foreach (JsonProperty jsonProperty in root.EnumerateObject())
                {
                    var name = jsonProperty.Name;
                    var node = root.GetProperty(name);
                    string type = node.ValueKind switch
                    {
                        JsonValueKind.String => "string",
                        JsonValueKind.Number => "int",
                        JsonValueKind.True => "bool",
                        JsonValueKind.False => "bool",
                        _ => "object",
                    };
                    string value = node.GetRawText();
                    dic.Add(jsonProperty.Name, (type, value));
                }

                var codeTemplate = new ProgramConstConfigCodeTemplate();
                codeTemplate.Session = new Dictionary<string, object>();
                codeTemplate.Session["GenerateLocation"] = DebugUtil.GetCallerContext();
                codeTemplate.Session["sigs"] = dic;
                var code = codeTemplate.TransformText();
                var path = config.ProgramConstConfigCSPath;
                
                //做个文件对比再决定是否写入。避免时间戳更新导致Unity刷新
                var oldCode = File.ReadAllText(path);
                if (oldCode != code) System.IO.File.WriteAllText(path, code);
            }

            #endregion
        }
    }
}