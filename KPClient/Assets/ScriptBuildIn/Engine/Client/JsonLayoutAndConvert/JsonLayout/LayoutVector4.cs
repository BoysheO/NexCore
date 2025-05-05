using System.Text.Json.Serialization;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.JsonLayout
{
    public struct LayoutVector4
    {
        [JsonIgnore] public Vector4 Primivate;

        public float x
        {
            get => Primivate.x;
            set => Primivate.x = value;
        }

        public float y
        {
            get => Primivate.y;
            set => Primivate.y = value;
        }

        public float z
        {
            get => Primivate.z;
            set => Primivate.z = value;
        }

        public float w
        {
            get => Primivate.w;
            set => Primivate.w = value;
        }
    }
}