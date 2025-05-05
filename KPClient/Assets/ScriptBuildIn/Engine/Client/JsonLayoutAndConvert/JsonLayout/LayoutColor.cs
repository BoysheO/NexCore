using System.Text.Json.Serialization;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.JsonLayout
{
    public struct LayoutColor
    {
        [JsonIgnore] public Color Primivate;

        public float r
        {
            get => Primivate.r;
            set => Primivate.r = value;
        }

        public float g
        {
            get => Primivate.g;
            set => Primivate.g = value;
        }

        public float b
        {
            get => Primivate.b;
            set => Primivate.b = value;
        }

        public float a
        {
            get => Primivate.a;
            set => Primivate.a = value;
        }
    }
}