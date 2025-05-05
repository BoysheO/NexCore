using System.Collections.Immutable;
using BoysheO.Extensions;
using TableDataGenerator.Model;
using TableDataGenerator.PbDataGeneratorSystem;

namespace TableDataGenerator;

/// <summary>
/// 构造ITableData接口
/// </summary>
public class ExternGenerator
{
    public static async Task GenerateInterfaceCodeAsync(IEnumerable<Book> books, IEnumerable<string> saveDirs,
        CancellationToken token,string filter = "")
    {
        var booksAry = books.ToArray();
        var dirs = saveDirs.ToArray();

        async ValueTask GenCode(Book book, CancellationToken token)
        {
            var fieldType = book.Sheets[0].GetColAttribute(ColOrder.FirstOrder, AttributeName.FieldType) ?? throw new Exception("missing type");
            if (fieldType != "int") throw new Exception("现在只支持int键作为key了");
            //获得这个表的id列的pb数据类型
            var keyType = PbType.FromTypeText(fieldType);
            if (keyType.IsAry) throw new NotSupportedException("can not use ary as key");
            //获得这个表id列的名字(一般来说都叫Id)
            var keyName = book.Sheets[0].GetColAttribute(ColOrder.FirstOrder, AttributeName.FieldName) ?? throw new Exception("missing name");
            //表里所有列的字段名
            var fieldNames = new List<string>();
            //表里所有列的CLR类型
            var fieldCLRTypes = new List<string>();
            //表里所有列的Include情况
            var fieldInclude = new List<bool>();

            //遍历excel表的第一个表的所有Attribute。这里Attribute是按列号-Attribute作为字典的
            foreach (var kvp in book.Sheets[0].Attributes)
            {
                //一个列有多个Attribute值，这里按Attribute名-Attribute值作为字典
                var atbName2colAtb = kvp.Value;
                ColOrder colOrder = kvp.Key;
                var fieldName = atbName2colAtb[AttributeName.FieldName];
                var fieldType1 = atbName2colAtb[AttributeName.FieldType];
                ColAttributes? tagStr = atbName2colAtb.TryGetValue(AttributeName.FieldTag,out var aab) ? aab:null;
                fieldNames.Add(fieldName.Value);
                var keyType1 = PbType.FromTypeText(fieldType1.Value);
                fieldCLRTypes.Add(keyType1.CLRType2);
                var isInclude = FilterHelper.ShouldBeIncluded(filter, tagStr.HasValue?tagStr.Value.Value:"", colOrder.Order == ColOrder.FirstOrder.Order);
                fieldInclude.Add(isInclude);
            }

            var codeTemplate = new PartialPart();
            codeTemplate.Session = new Dictionary<string, object>();
            codeTemplate.Session["TypeName"] = book.Name + "Table";
            codeTemplate.Session["KeyType"] = keyType.CLRType;
            codeTemplate.Session["KeyName"] = keyName;
            codeTemplate.Session["FieldNames"] = fieldNames.ToArray();
            codeTemplate.Session["FieldCLRTypes"] = fieldCLRTypes.ToArray();
            codeTemplate.Session["FieldInclude"] = fieldInclude.ToArray();
            var code = codeTemplate.TransformText();
            foreach (var saveDir in dirs)
            {
                var file = saveDir + "/" + book.Name.MakeFirstCharUpperOrNot() + "Table.ITableData.cs";
                await File.WriteAllTextAsync(file, code, token);
            }
        }

        //await Parallel.ForEachAsync(booksAry, token, GenCode);
        foreach (var book in booksAry)
        {
            await GenCode(book, token);
        }
    }
}