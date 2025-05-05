using BoysheO.Extensions;

namespace TableDataGenerator;

public static class FilterHelper
{
    public static bool ShouldBeIncluded(string filter, string tagStr, bool isIdCol)
    {
        if (isIdCol) return true; //id列必须被包含
        filter = filter.Trim();
        var tags = tagStr.Split(',').Select(v => v.Trim()).Where(v => !v.IsNullOrWhiteSpace()).ToArray();
        return filter.IsNullOrWhiteSpace() || tags.Length == 0 || tags.Contains(filter);
    }
}