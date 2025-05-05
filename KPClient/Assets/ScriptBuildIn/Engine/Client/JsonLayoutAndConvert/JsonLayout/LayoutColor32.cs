using System.Text.Json.Serialization;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.JsonLayout
{
    public struct LayoutColor32
    {
        [JsonIgnore] public Color32 Primivate;

        public byte r
        {
            get => Primivate.r;
            set => Primivate.r = value;
        }

        public byte g
        {
            get => Primivate.g;
            set => Primivate.g = value;
        }

        public byte b
        {
            get => Primivate.b;
            set => Primivate.b = value;
        }

        public byte a
        {
            get => Primivate.a;
            set => Primivate.a = value;
        }
    }
}