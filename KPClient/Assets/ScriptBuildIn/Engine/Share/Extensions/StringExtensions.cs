namespace ScriptEngine.BuildIn.ShareCode.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 这个函数命名为ToLan纯碎是为了方便替换成int.ToLan。
        /// ###是约定的多语言标记开始
        /// </summary>
        public static string ToLan(this string str)
        {
            if (str.StartsWith("###"))
            {
                return str.Substring(3, str.Length - 3);
            }

            return str;
        } 
    }
}