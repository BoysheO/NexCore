using System.Text.Json.Serialization;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.JsonLayout
{
    public struct LayoutVector2
    {
        [JsonIgnore] public Vector2 Primivate;

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
    }
}