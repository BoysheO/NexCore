using System.Text;
using BoysheO.Extensions;
using BoysheO.Util;
using TableDataGenerator.Model;
using TableDataGenerator.PbDataGeneratorSystem;
using WorkFlow;

namespace TableDataGenerator;

public class PbModelGenerator
{
    public static async Task Gen(IEnumerable<string> saveDirs, IEnumerable<Book> books, CancellationToken token,
        ProtoToolsInformation protoToolsInformation, string genFilter)
    {
        if (protoToolsInformation == null) throw new ArgumentNullException(nameof(protoToolsInformation));
        //临时生成的.proto文件存放位置
        var protoFileOutputDir = Path.GetTempPath() + "" + Random.Shared.NextInt64();
        while (Directory.Exists(protoFileOutputDir))
        {
            protoFileOutputDir = Path.GetTempPath() + "/" + Random.Shared.NextInt64();
        }

        Directory.CreateDirectory(protoFileOutputDir);

        await Parallel.ForEachAsync(books, token, async (book, token) =>
        {
            var codeTemplate = new ProtoT4();
            codeTemplate.Session = new Dictionary<string, object>();
            codeTemplate.Session["PckName"] = book.Name.MakeFirstCharLowerOrNot();
            codeTemplate.Session["GenerateLocation"] = DebugUtil.GetCallerContext();
            codeTemplate.Session["TableName"] = book.Name;
            var sheet = book.Sheets[0];

            codeTemplate.Session["ColumNames"] = sheet.Attributes.Select(v => v.Value[AttributeName.FieldName])
                .Select(v => v.Value.MakeFirstCharLowerOrNot()).ToArray();

            var types = sheet.Attributes
                .Select(v => v.Value[AttributeName.FieldType])
                .Select(v =>
                {
                    try
                    {
                        return PbType.FromTypeText(v.Value);
                    }
                    catch
                    {
                        Console.WriteLine($"fail at book={book.Name} sheet={sheet.Name} col={v.Name.Name}");
                        throw;
                    }
                }).ToArray();
            var comments = sheet.Attributes
                .Select(v =>
                {
                    var a = v.Value.GetValueOrDefault(AttributeName.Comment);
                    return a.Value ?? "";
                }).ToArray();

            var isInclude = sheet.Attributes
                .Select(v =>
                {
                    var tag = v.Value.GetValueOrDefault(AttributeName.FieldTag);
                    return tag.Value ?? "";
                })
                .Select((v, i) =>
                {
                    var isInclude = FilterHelper.ShouldBeIncluded(genFilter, v, i == 0);
                    return isInclude;
                }).ToArray();

            codeTemplate.Session["ColumTypes"] = types.Select(v => v.Prefix).ToArray();
            codeTemplate.Session["ColumIsAry"] = types.Select(v => v.IsAry).ToArray();
            //codeTemplate.Session["ColumComment"] = new string[types.Length]; //懒得实现注释了
            codeTemplate.Session["ColumComment"] = comments;
            codeTemplate.Session["ColumIsIncluded"] = isInclude;
            var code = codeTemplate.TransformText();
            await File.WriteAllTextAsync(protoFileOutputDir + "/" + book.Name + "Table.proto", code, Encoding.UTF8,
                token);
        });

        Console.WriteLine($"proto gen at {protoFileOutputDir}");

        #region gen code

        foreach (var saveDir in saveDirs)
        {
            //delete all cs file
            if (Directory.Exists(saveDir))
            {
                foreach (var s in Directory.EnumerateFiles(saveDir)
                             .Where(v => v.AsPath().GetExtension() == ".cs"))
                {
                    File.Delete(s);
                }
            }

            await ProtocUtil.GenProto(protoFileOutputDir, Array.Empty<string>(), saveDir, false, protoToolsInformation);
        }


        #endregion
        
        //清理临时proto文件夹
        Directory.Delete(protoFileOutputDir, true);
    }
}