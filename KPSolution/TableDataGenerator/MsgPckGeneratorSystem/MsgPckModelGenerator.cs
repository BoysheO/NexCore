// using TableDataGenerator.Model;
// using BoysheO.Util;
// using BoysheO.Extensions;
// using System.Collections.Immutable;
// using TableDataGenerator.DataParser;
// using TableDataGenerator.MsgPckGeneratorSystem;
//
// // using TableDataGenerator.MsgPckGeneratorSystem;
//
// namespace TableDataGenerator
// {
//     public static class MsgPckModelGenerator
//     {
//         public static void Gen(DirectoryInfo saveDir, IEnumerable<Book> books)
//         {
//             if (saveDir.Exists)
//             {
//                 saveDir.Delete(true);
//             }
//             saveDir.Create();
//
//             var dataParsers = new BaseParser();
//             string ConverType(string rawType)
//             {
//                 return !dataParsers.IsTypeStrMatch(rawType,out var type) ? throw new Exception($"not support data type={rawType}") : type;
//             }
//
//             foreach (var book in books)
//             {
//                 foreach (var sheet in book.Sheets)
//                 {
//                     var codeTemplate = new ModelCodeTemplate();
//                     codeTemplate.Session = new Dictionary<string, object>();
//                     codeTemplate.Session["GenerateLocation"] = DebugUtil.GetCallerContext();
//                     var modelName = book.Name + "Table";
//                     codeTemplate.Session["ModelName"] = modelName;
//                     codeTemplate.Session["FieldTypes"] = sheet.Attributes.Select(v => v.Value[AttributeName.FieldType].Value)
//                         .Select(v => ConverType(v)).ToArray();
//                     codeTemplate.Session["FieldName"] = sheet.Attributes.Select(v => v.Value[AttributeName.FieldName].Value).ToArray();
//                     var code = codeTemplate.TransformText();
//                     File.WriteAllText(saveDir.FullName + "/" + $"{modelName}.cs", code);
//                 }
//             }
//         }
//     }
// }
