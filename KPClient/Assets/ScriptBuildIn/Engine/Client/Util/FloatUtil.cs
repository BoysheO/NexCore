namespace Framework.Hotfix.SharedCode.Customer.Util{

public static class FloatUtil
{
    /// <summary>
    /// 等同 高版本C#的float.IsFinite
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsFinite(float value)
    {
        int i;
        unsafe
        {
            i = *(int*) &value;
        }

        var isFinite = (i & int.MaxValue) < 2139095040;
        return isFinite;
    }
}}