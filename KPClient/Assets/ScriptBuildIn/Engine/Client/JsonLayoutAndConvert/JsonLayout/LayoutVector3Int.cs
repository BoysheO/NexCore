using System.Text.Json.Serialization;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.JsonLayout
{
    public struct LayoutVector3Int
    {
        [JsonIgnore] public Vector3Int Primivate;

        public int x
        {
            get => Primivate.x;
            set => Primivate.x = value;
        }

        public int y
        {
            get => Primivate.y;
            set => Primivate.y = value;
        }

        public int z
        {
            get => Primivate.z;
            set => Primivate.z = value;
        }
    }
}