using System.Text.Json.Serialization;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.JsonLayout
{
    public struct LayoutBounds
    {
        [JsonIgnore] public Bounds Primivate;

        public Vector3 center
        {
            get => Primivate.center;
            set => Primivate.center = value;
        }

        public Vector3 extents
        {
            get => Primivate.extents;
            set => Primivate.extents = value;
        }
    }
}