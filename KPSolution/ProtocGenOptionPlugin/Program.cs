/*
 * 本来想用自定义plugin的方式来生成代码，以实现proto根据genOption来决定是否生成对应代码。
 * 但是实际上发现这样是无法修改protoc生成的字段属性的，除非覆盖protoc的chsarpoutput，这样的话工作量很大
 * 还要维护这个代码生成程序
 *
 * 思量再三，还是从proto文件修改下手，因此弃用这个代码生成器。作为示例学习使用，这个生成器会保留。
 */
using Google.Protobuf;
using Google.Protobuf.Compiler;

namespace ProtocGenOptionPlugin;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        // 从标准输入读取 CodeGeneratorRequest
        var request = CodeGeneratorRequest.Parser.ParseFrom(Console.OpenStandardInput());
        var response = new CodeGeneratorResponse();

        // 处理每个 proto 文件
        foreach (var protoFile in request.ProtoFile)
        {
            // 示例：生成一个简单的 C# 文件内容
            var fileContent = $"// Auto-generated from {protoFile.Name}\n\n";
            fileContent += "using System;\n\n";
            fileContent += "namespace GeneratedNamespace {\n";
            fileContent += "    public class ExampleClass {\n";
            fileContent += "        public void HelloWorld() {\n";
            fileContent += "            Console.WriteLine(\"Hello, World!\");\n";
            fileContent += "        }\n";
            fileContent += "    }\n";
            fileContent += "}\n";

            // 创建 CodeGeneratorResponse.File 对象
            var file = new CodeGeneratorResponse.Types.File
            {
                Name = $"{Path.GetFileNameWithoutExtension(protoFile.Name)}.g.cs",
                Content = fileContent
            };

            response.File.Add(file);
        }

        // 将 CodeGeneratorResponse 输出到标准输出
        using (var output = Console.OpenStandardOutput())
        {
            response.WriteTo(output);
        }
    }
}