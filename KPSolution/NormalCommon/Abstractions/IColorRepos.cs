using System.Collections.Generic;

namespace NormalCommon.Abstractions
{
    public interface IColorRepos
    {
        (float r, float g, float b, float a)[] Colors { get; }
    }
}