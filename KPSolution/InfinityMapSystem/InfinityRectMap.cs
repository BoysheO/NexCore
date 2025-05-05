// using System;
// using System.Threading.Tasks;
// using Cysharp.Collections;
// using MathNet.Numerics.LinearAlgebra;
// using MathNet.Numerics.LinearAlgebra.Complex;
//
// namespace InfinityMapSystem
// {
//     /**
//      * 无限地图
//      * 读写API仍按xy坐标系进行读写。储存上按4象限存放以便于地图拓展长宽
//      * MipMap支持，按10x10的区块进行MipMap
//      * 多线程支持，读写分离
//      * 但是现在暂时按普通数组设计，以快速原型
//     */
//     public class InfinityRectMap<T> where T : unmanaged, IEquatable<T>, IFormattable
//     {
//         private NativeMemoryArray<T> _data;
//         private readonly object _gate = new object();
//
//         public int Width { get; private set; }
//         public int Height { get; private set; }
//
//         public int CenterX { get; private set; }
//
//         public int CenterY { get; private set; }
//
//         public InfinityRectMap(int centerX, int centerY, int width, int height)
//         {
//             if (width % 2 != 0) width++;
//             if (height % 2 != 0) height++;
//             _data = new NativeMemoryArray<T>(width * height);
//             Width = width;
//             Height = height;
//             CenterY = centerY;
//             CenterX = centerX;
//         }
//
//         public void Resize(int width, int height)
//         {
//             lock (_gate)
//             {
//                 var newData = new NativeMemoryArray<T>(width * height);
//                 for (int x = 0; x < Width; x++)
//                 {
//                     for (int y = 0; y < Height; y++)
//                     {
//                         newData[x + y * width] = _data[x + y * Width];
//                     }
//                 }
//
//                 _data.Dispose();
//                 _data = newData;
//             }
//         }
//
//         public T this[int x, int y]
//         {
//             get { return _data[x + y * Width]; }
//             set { _data[x + y * Width] = value; }
//         }
//
//         public InfinityRectMap<T> GetRange(int x, int y, int width, int height)
//         {
//         }
//     }
// }