using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoysheO.Extensions;
using Microsoft.Extensions.Logging;
using TableDataGenerator.Model;

namespace TableDataGenerator
{
    public static class ExcelReader
    {
        public static async Task<Book> BuildAsync(FileInfo file)
        {
            using var ms = new MemoryStream();
            using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                await fs.CopyToAsync(ms);
            }

            ms.Seek(0, SeekOrigin.Begin);
            using ExcelPackage excel = new ExcelPackage(ms);
            var ebook = new LinqForEEPlus.Book(excel.Workbook, file.FullName);
            var sheetBuilder = ImmutableArray.CreateBuilder<Sheet>();
            var esheet = ebook[0];

            var sheetAttbutesBuilder = new Sheet.SheetAttributesBuilder();
            //esheet索引是行号，行号从1开始
            var commentRow = esheet[1]; //第一行为注释行
            var tagRow = esheet[2]; //第二行为tag行，一般标记c,s来表达是否都生成
            var typeRow = esheet[3]; //类型行，具体支持类型见pbType实现
            var nameRow = esheet[4]; //字段名行
            //这里储存的列号，范围为[0,+)
            List<int> colsValid = new List<int>();

            //从1开始循环是因为esheet将第1列视作colNum=1，colNum=0是不存在的。
            for (int colnum = 1; colnum <= esheet.ColMax; colnum++)
            {
                var isIdCol = colnum == 1;
                var type = typeRow[colnum];
                if (!isIdCol && !type.IsValuable) continue; //没有写类型的列忽略
                var name = nameRow[colnum];
                if (!isIdCol && !name.IsValuable) continue; //没有写名字的列忽略
                var comment = commentRow[colnum]; //注释，注释是可选的
                var tag = tagRow[colnum]; //tag，tag是可选的

                //对于id列，强制使用int来替换它的任何type值
                sheetAttbutesBuilder.Add(colnum - 1, AttributeName.FieldType.Name, isIdCol ? "int" : type.StrValue);
                sheetAttbutesBuilder.Add(colnum - 1, AttributeName.FieldName.Name, name.StrValue);
                // @formatter:off
                if (!isIdCol && tag.IsValuable) sheetAttbutesBuilder.Add(colnum - 1, AttributeName.FieldTag.Name, tag.StrValue);
                if (comment.IsValuable) sheetAttbutesBuilder.Add(colnum - 1, AttributeName.Comment.Name, comment.StrValue);
                // @formatter:on
                colsValid.Add(colnum - 1);
            }

            var rowsBuilder = ImmutableArray.CreateBuilder<Model.Row>();
            //第一行是注释行，故忽略
            //第二行是tag行，忽略
            //第三行是类型行
            //第四行是列名行
            //从第五行开始才是内容行。esheet里面第一行是1而不是0。因此遍历从5开始
            for (int row = 5; row <= esheet.RowMax; row++)
            {
                var rowBuilder = Model.Row.CreatBuilder(row);
                foreach (var col in colsValid)
                {
                    //由于esheet的col是[1,+)，所以这里将col转换下，要+1
                    var cell = esheet[row, col + 1];
                    rowBuilder.Add(col, cell.StrValue);
                }

                var rr = rowBuilder.Build();
                //不论什么情况，第一列为空，即没有配id，这行都舍弃
                if (!rr.Values.TryGetValue(ColOrder.FirstOrder, out var v) || v.IsNullOrWhiteSpace()) continue;
                rowsBuilder.Add(rr);
            }

            Sheet mySheet = new Sheet(esheet.Name, sheetAttbutesBuilder.Build(), rowsBuilder.ToImmutable());
            sheetBuilder.AddRange(mySheet);
            var modelBook = new Book(ebook.ShortFileName.Split('.')[0], sheetBuilder.ToImmutable());
            return modelBook;
        }

        public static async Task<ImmutableArray<Book>> BuildAsync(ILogger logger, params DirectoryInfo[] directory)
        {
            var builder = ImmutableArray.CreateBuilder<Book>();
            foreach (var directoryInfo in directory)
            {
                if (directoryInfo.Exists)
                {
                    foreach (var file in directoryInfo.EnumerateFiles())
                    {
                        if (file.Extension != ".xlsx") continue;
                        if (file.Name.StartsWith("~$")) continue; //win
                        if (file.Name.StartsWith(".~")) continue; //mac

                        var book = await BuildAsync(file);
                        builder.Add(book);
                    }
                }
                else
                {
                    logger.LogError("Skip directory {dir} due to dir is not exist", directoryInfo.FullName);
                }
            }

            return builder.ToImmutable();
        }
    }
}