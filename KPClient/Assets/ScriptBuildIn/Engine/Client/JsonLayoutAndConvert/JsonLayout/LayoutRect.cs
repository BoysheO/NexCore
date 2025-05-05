using System.Text.Json.Serialization;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.JsonLayout
{
    public struct LayoutRect
    {
        [JsonIgnore] public Rect Primivate;

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

        public float width
        {
            get => Primivate.width;
            set => Primivate.width = value;
        }

        public float height
        {
            get => Primivate.height;
            set => Primivate.height = value;
        }
    }
}