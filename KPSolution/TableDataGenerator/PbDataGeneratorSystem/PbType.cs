using System.Runtime.ExceptionServices;
using BoysheO.Extensions;

namespace TableDataGenerator;

public class PbType
{
    private static readonly SortedSet<string> SupportPrefix = new SortedSet<string>()
    {
        "int32",
        "int64",
        "double",
        "bool",
        "string",
    };

    /// <summary>
    /// 别名转换
    /// </summary>
    private static readonly Dictionary<string, string[]> AnotherName = new()
    {
        ["int32"] = new string[] {"int"},
        ["int64"] = new string[] {"long"},
        ["bool"] = new string[] {"boolean"},
        ["double"] = new string[] {"float"},
    };

    public bool IsAry { get; init; }

    /// <summary>
    /// 只可能是这几种情况 int32 int64 double bool string
    /// </summary>
    public string Prefix { get; init; }

    /// <summary>
    /// 返回int long double bool string 其中之一
    /// </summary>
    public string CLRType
    {
        get
        {
            string result;
            switch (Prefix)
            {
                case "int32":
                    result = "int";
                    break;
                case "int64":
                    result = "long";
                    break;
                case "double":
                case "bool":
                case "string":
                    result = Prefix;
                    break;
                default:
                    throw new NotSupportedException(Prefix);
            }

            return result;
        }
    }

    /// <summary>
    /// 返回 int long double bool string RepeatedField{int} RepeatedField{long} ...其中之一
    /// </summary>
    public string CLRType2
    {
        get
        {
            if (!IsAry) return CLRType;
            return $"RepeatedField<{CLRType}>";
        }
    }

    /// <summary>
    /// 根据文本，生成CLR运行时值
    /// </summary>
    public object ConvertValue(string valueStr)
    {
        if (IsAry)
        {
            if (valueStr.IsNullOrWhiteSpace())
            {
                switch (Prefix)
                {
                    case "int32":
                        return Array.Empty<int>();
                    case "int64":
                        return Array.Empty<long>();
                    case "double":
                        return Array.Empty<double>();
                    case "bool":
                        return Array.Empty<bool>();
                    case "string":
                        return Array.Empty<string>();
                    default:
                        throw new Exception($"unsupport type={Prefix}");
                }
            }
            else
            {
                var sp = valueStr.Split(";");
                switch (Prefix)
                {
                    case "int32":
                        return sp.Where(v => !v.IsNullOrWhiteSpace()).Select(v => (int) this.ParseSingleValue(v))
                            .ToArray();
                        break;
                    case "int64":
                        return sp.Where(v => !v.IsNullOrWhiteSpace()).Select(v => (long) this.ParseSingleValue(v))
                            .ToArray();
                    case "double":
                        return sp.Where(v => !v.IsNullOrWhiteSpace()).Select(v => (double) this.ParseSingleValue(v))
                            .ToArray();
                    case "bool":
                        return sp.Where(v => !v.IsNullOrWhiteSpace()).Select(v => (bool) this.ParseSingleValue(v))
                            .ToArray();
                    case "string":
                        return sp.Where(v => !v.IsNullOrWhiteSpace()).Select(v => (string) ParseSingleValue(v))
                            .ToArray();
                    default:
                        throw new Exception($"unsupport type={Prefix}");
                }
            }
        }
        else
        {
            return ParseSingleValue(valueStr);
        }
    }

    private object ParseSingleValue(string valueStr)
    {
        try
        {
            switch (Prefix)
            {
                case "int32":
                    return valueStr.IsNullOrEmpty() ? 0 : int.Parse(valueStr);
                    break;
                case "int64":
                    return valueStr.IsNullOrEmpty() ? 0 : Int64.Parse(valueStr);
                    break;
                case "double":
                    return valueStr.IsNullOrEmpty() ? 0 : Double.Parse(valueStr);
                    break;
                case "bool":
                {
                    if (valueStr.IsNullOrEmpty()) return false;
                    if (Boolean.TryParse(valueStr, out var b)) return b;
                    if (int.TryParse(valueStr, out var v))
                    {
                        if (v == 0) return false;
                        if (v == 1) return true;
                    }

                    throw new Exception($"value={valueStr} not a boolean");
                    break;
                }
                case "string":
                    return valueStr ?? "";
                default:
                    throw new Exception($"invalid type={Prefix ?? "null"}");
                    break;
            }
        }
        catch (Exception ex)
        {
            //Throws the source exception, maintaining the original Watson information and augmenting rather than replacing the original stack trace.
            throw new Exception($"covert fail.str={valueStr}", ex);
            //ExceptionDispatchInfo.Throw(ex);
        }
    }

    public static PbType FromTypeText(string rawTypeText)
    {
        var rawText = rawTypeText.Trim();
        bool isAry = rawText.EndsWith("[]");
        string prefix;
        if (isAry) prefix = rawText[..^2];
        else prefix = rawText;
        prefix = prefix.ToLower();
        prefix = CovertToStandardNameOrKeepOriginal(prefix);
        if (!IsPrefixSupported(prefix)) throw new Exception($"not support prefix.raw={rawTypeText}");
        return new PbType()
        {
            IsAry = isAry,
            Prefix = prefix
        };
    }

    private static string CovertToStandardNameOrKeepOriginal(string raw)
    {
        foreach (var (key, value) in AnotherName)
        {
            if (value.Contains(raw)) return key;
        }

        return raw;
    }

    private static bool IsPrefixSupported(string prefix)
    {
        return SupportPrefix.Contains(prefix);
    }
}