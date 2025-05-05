using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Reflection;
using System.Threading.Tasks;
using BoysheO.Extensions;
using Google.Protobuf;

namespace HotScripts.Hotfix.SharedCode.HotfixCommon.ProtobufTableSystem
{
    public static class ResolveUtil
    {
        public static T[] Resolve<T>(byte[] bytes)
        {
            if (!typeof(T).IsClassAndImplement(typeof(Google.Protobuf.IMessage)))
                throw new Exception("pb support only");


            // BagData.Parser
            var parserProperty = typeof(T).GetProperty("Parser", BindingFlags.Static | BindingFlags.Public) ??
                                 throw new Exception("unknown");
            var parser = parserProperty.GetValue(null) as MessageParser ?? throw new Exception("unknown");

            #region 解析

            int seek = 0;
            var count = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(seek, sizeof(int)));
            if (count < 0)
            {
                throw new Exception($"invalid count={count}");
            }

            seek += sizeof(int);
            int[] offsetAry = ArrayPool<int>.Shared.Rent(count);
            int[] bytesLenAry = ArrayPool<int>.Shared.Rent(count);
            for (int i = 0; i < count; i++)
            {
                var ll = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(seek, sizeof(int)));
                seek += sizeof(int);
                offsetAry[i] = seek;
                bytesLenAry[i] = ll;
                seek += ll;
            }

            T[] ary = new T[count];

            Parallel.For(0, count, i =>
            {
                var offset = offsetAry[i];
                var len = bytesLenAry[i];
                var bytes1 = bytes.AsSpan(offset, len);
                var ins = (T) parser.ParseFrom(bytes1);
                ary[i] = ins;
            });

            ArrayPool<int>.Shared.Return(offsetAry);
            ArrayPool<int>.Shared.Return(bytesLenAry);

            #endregion

            return ary;
        }
    }
}