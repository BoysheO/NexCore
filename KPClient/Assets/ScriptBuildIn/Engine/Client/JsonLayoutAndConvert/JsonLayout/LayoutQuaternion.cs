using System.Text.Json.Serialization;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.JsonLayout
{
    public struct LayoutQuaternion
    {
        [JsonIgnore] public Quaternion Primivate;

        public float x
        {
            get => Primivate.x;
            set => Primivate.y = value;
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