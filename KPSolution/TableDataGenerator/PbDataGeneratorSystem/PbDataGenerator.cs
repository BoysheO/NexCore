using System.Buffers.Binary;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using BoysheO.Extensions;
using Google.Protobuf;
using TableDataGenerator.Model;
using WorkFlow;

namespace TableDataGenerator;

public class PbDataGenerator
{
    public static async Task Gen(IEnumerable<string> saveBytesToDirs, IEnumerable<Book> books,
        string pbCodeDirForGenAsb, CancellationToken token, string? projectFileOverload = null, string filter = "")
    {
        projectFileOverload ??= ProjectFileContent;
        var saveBytesToDirsAry = saveBytesToDirs.ToArray();

        #region 生成程序集并加载
        
        #region generate csproj

        var csProj = pbCodeDirForGenAsb + "/TempLibrary.csproj";
        await File.WriteAllTextAsync(csProj, projectFileOverload, token);

        #endregion

        var (dllPath, pdbPath, xmlPath) =
            await CSProjectUtil.BuildProjectByDotnetCommandAsync2(new FileInfo(csProj), false, "netstandard2.0", false);

        //需要本工程引用google.protobuf包才能成功load
        var asb = Assembly.LoadFile(dllPath);
        var types = asb.GetTypes().Where(v => v.IsClassAndImplement(typeof(IMessage)));
        Console.WriteLine("——————成功识别的类型有这些————————");
        foreach (var type in types)
        {
            Console.WriteLine(type.Name);
        }

        Console.WriteLine("——————end成功识别的类型————————");

        #endregion

        #region clear dirs

        await Parallel.ForEachAsync(saveBytesToDirsAry, token, async (saveBytesToDir, v) =>
        {
            if (Directory.Exists(saveBytesToDir)) Directory.Delete(saveBytesToDir, true);
            Directory.CreateDirectory(saveBytesToDir);
            await File.WriteAllTextAsync(saveBytesToDir + "/.gitignore", gitIgnore, v);
        });

        #endregion

        #region 生成bytes

        async ValueTask GenTableBytes(Book book, CancellationToken cancellationToken)
        {
            var tableName = book.Name;
            var typeName = tableName + "Table";
            var type = asb.GetTypes().First(v => v.Name == typeName);
            var sheet = book.Sheets[0];
            ConcurrentBag<byte[]> rowBytes = new();

            async ValueTask GenRowData(Row row, CancellationToken cancellationToken)
            {
                var ins = Activator.CreateInstance(type) ?? throw new Exception("creat ins fail");
                await Parallel.ForEachAsync(row.Values, cancellationToken, async (kvp, _) =>
                {
                    var colOrder = kvp.Key;
                    var value = kvp.Value;
                    var tagStr = sheet.GetColAttribute(colOrder, AttributeName.FieldTag) ?? "";
                    var isInclude = FilterHelper.ShouldBeIncluded(filter, tagStr, row.Order == 1);
                    if (!isInclude) return;
                    try
                    {
                        var fieldType = sheet.GetColAttribute(colOrder, AttributeName.FieldType) ??
                                        throw new Exception($"missing field type.");
                        var fieldName = sheet.GetColAttribute(colOrder, AttributeName.FieldName) ??
                                        throw new Exception($"missing fieldName.");
                        var pbType = PbType.FromTypeText(fieldType);
                        var propertyName = fieldName.MakeFirstCharUpperOrNot();
                        var propertyValue = pbType.ConvertValue(value);
                        var property = type.GetProperty(propertyName);
                        //丢失属性是指表里面有这个列，但是生成的代码里这个列对应的C#Property没有或者被注释掉了
                        //这种情况发生，有可能是最近修改过生成模块导致数据和Model对不上，有可能是filter和模块生成没对上
                        property = property ?? throw new Exception($"missing property in code.");
                        if (!pbType.IsAry)
                        {
                            property.SetValue(ins, propertyValue);
                        }
                        else
                        {
                            var lst = property.GetValue(ins) as IList ?? throw new Exception("dismatch type");
                            var r = propertyValue as Array ?? throw new Exception("");
                            foreach (var o in r)
                            {
                                lst.Add(o);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(
                            $"ex occur.book={book.Name},row={row.Order},col={colOrder.Order},{colOrder.ToExcelColumLetters()},colName={sheet.GetColAttribute(colOrder, AttributeName.FieldName)}",
                            exception);
                    }
                });
                var msg = ins as IMessage ?? throw new Exception("cover fail");
                var bytes = msg.ToByteArray();
                rowBytes.Add(bytes);
            }

            // await Parallel.ForEachAsync(sheet.Rows, cancellationToken, GenRowData);
            foreach (var sheetRow in sheet.Rows)
            {
                await GenRowData(sheetRow, cancellationToken);
            }

            using MemoryStream mem = new MemoryStream();
            var buff = new byte[sizeof(int)];
            //写入总行数，以便于读表时可以创建合适的Buffer大小
            BinaryPrimitives.WriteInt32BigEndian(buff, rowBytes.Count);
            mem.Write(buff);
            //按 行bytes长度 + bytes 写入
            foreach (var rowByte in rowBytes)
            {
                var len = rowByte.Length;
                BinaryPrimitives.WriteInt32BigEndian(buff, len);
                mem.Write(buff);
                mem.Write(rowByte);
            }

            foreach (var saveBytesToDir in saveBytesToDirsAry)
            {
                var fsPath = saveBytesToDir + "/" + typeName + ".bytes";
                await using var fs = new FileStream(fsPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                mem.Seek(0, SeekOrigin.Begin);
                await mem.CopyToAsync(fs, cancellationToken);
                Console.WriteLine($"{fsPath}已写入{fs.Length}");
            }
        }

        // await Parallel.ForEachAsync(books, token, GenTableBytes);
        foreach (var book in books)
        {
            await GenTableBytes(book, token);
        }

        #endregion
    }

    private const string ProjectFileContent = """
                                              <Project Sdk="Microsoft.NET.Sdk">
                                              
                                                  <PropertyGroup>
                                                      <TargetFramework>netstandard2.0</TargetFramework>
                                                  </PropertyGroup>
                                              
                                                  <ItemGroup>
                                                    <PackageReference Include="Google.Protobuf" Version="3.25.1" />
                                                  </ItemGroup>

                                              </Project>

                                              """;

    private const string gitIgnore = """
                                     *.bytes
                                     *.meta
                                     """;
}