using System.Buffers;
using System.Runtime.CompilerServices;
using Extensions;
using GameFramework;
using TMPro;

namespace Hotfix.Extensions
{
    public static class TextMeshProUguiExtensions
    {
        //private static readonly ConditionalWeakTable<TextMeshProUGUI, char[]> _tmp2charBuffer = new();

        public static void SetCharBuffer(this TextMeshProUGUI tmp, zstring str)
        {
            if (str.Length == 0)
            {
                tmp.SetText("");
                return;
            }

            // if (!_tmp2charBuffer.TryGetValue(tmp, out var charBuffer))
            // {
            //     charBuffer = ArrayPool<char>.Shared.Rent(str.Length);
            //     _tmp2charBuffer.Add(tmp, charBuffer);
            // }
            //
            // if (charBuffer.Length < str.Length)
            // {
            //     charBuffer = ArrayPoolUtil.Resize(charBuffer, str.Length);
            //     _tmp2charBuffer.AddOrUpdate(tmp, charBuffer);
            // }

            var charBuffer = ArrayPool<char>.Shared.Rent(str.Length);
            str.ToString().CopyTo(0, charBuffer, 0, str.Length);
            tmp.SetCharArray(charBuffer, 0, str.Length);
            ArrayPool<char>.Shared.Return(charBuffer);
        }
    }
}