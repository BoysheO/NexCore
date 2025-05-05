#nullable disable
namespace ServicesCommon.Manager;

public class TableBytesManifestDto
{
    public sealed class Entry
    {
        //纯表名。ex："config"
        public string TableName { get; set; }

        //纯文件名。ex："configTable.bytes"  "a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3.bytes"
        public string FileName { get; set; }
    }

    public List<Entry> FileNames { get; set; } = new();
}