// using BoysheO.Extensions;
// using MessagePack;
// using SolutionConfigSpace;
// using System.Diagnostics;
// using System.Reflection;
// using System.Text;
// using TableDataGenerator.DataParser;
// using TableDataGenerator.Model;
//
// namespace TableDataGenerator
// {
//     public static class MsgPckDataGenerator
//     {
//         public static void Gen(IEnumerable<string> saveDirs, IEnumerable<Book> books)
//         {
//             var config = new SolutionConfig();
//             var modelProject = config.TableModelProjectFile;
//             //todo:多平台支持
//             Process process = new Process();
//             process.StartInfo.FileName = "dotnet";
//             process.StartInfo.Arguments = $"build {modelProject} -c Release";
//             process.StartInfo.RedirectStandardOutput = true;
//             process.StartInfo.StandardOutputEncoding = Encoding.UTF8; // 指定编码
//             process.Start();
//             process.WaitForExit();
//             string output = process.StandardOutput.ReadToEnd();
//             Console.WriteLine(output);
//             Console.WriteLine("build done.");
//             //D:\Repos\TGCGame3\TGCGameOthers\TableData\bin\Release\netstandard2.0\TableModels.dll
//             var modelProjectDir = modelProject.Directory ?? throw new NullReferenceException();
//             var asb = Assembly.LoadFile(@$"{modelProjectDir.FullName}\bin\Release\netstandard2.0\TableModels.dll");
//             Console.WriteLine("已加载Table程序集包含的类型：\n" + asb.GetTypes().Select(v => v.Name).JoinAsOneString("\n"));
//
//             foreach (var saveDir in saveDirs)
//             {
//                 if (Directory.Exists(saveDir))
//                 {
//                     Directory.Delete(saveDir, true);
//                 }
//
//                 Directory.CreateDirectory(saveDir);
//             }
//
//
//             var dataParser = new BaseParser();
//             foreach (var book in books)
//             {
//                 var sheet = book.Sheets[0];
//                 var typeName = book.Name + "Table";
//                 var type = asb.GetTypes().First(typ => typ.Name == typeName);
//                 var ctors = type.GetConstructors().First();
//                 List<object> lst = new List<object>();
//                 foreach (var row in sheet.Rows)
//                 {
//                     List<object> args = new List<object>();
//                     foreach (var v in row.Values)
//                     {
//                         var fieldType = sheet.GetColAttribute(v.Key, AttributeName.FieldType) ??
//                                         throw new Exception($"missing field type.book={book.Name},row={row.Order}");
//                         var fieldName = sheet.GetColAttribute(v.Key, AttributeName.FieldName);
//                         var data = v.Value;
//                         if (!dataParser.IsTypeStrMatch(fieldType, out var typeCode))
//                             throw new Exception($"invalid fieldType={fieldType}.book={book.Name},row={row.Order}");
//                         try
//                         {
//                             var value = dataParser.Convert(data, typeCode);
//                             args.Add(value);
//                         }
//                         catch (Exception exception)
//                         {
//                             throw new Exception(
//                                 $"convert data error.book={book.Name},row={row.Order}.ex={exception.Message}",
//                                 exception);
//                         }
//                     }
//
//                     try
//                     {
//                         var ins = ctors.Invoke(args.ToArray());
//                         lst.Add(ins);
//                     }
//                     catch (Exception exception)
//                     {
//                         //如果这里报错，有可能是因为表格模型程序集中包含了一些编译错误，这些错误通常是由自定义的分部类引起的
//                         throw;
//                     }
//                 }
//
//                 var bytes = MessagePackSerializer.Serialize<List<object>>(lst);
//                 foreach (var dir in saveDirs)
//                 {
//                     File.WriteAllBytes(@$"{dir}\{typeName}.bytes", bytes);
//                 }
//             }
//         }
//     }
// }