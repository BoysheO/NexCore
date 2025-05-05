using System;
using System.Text.Json.Serialization;
using BoysheO.Extensions.Unity3D.Extensions;
using BoysheO.Extensions.Unity3D.Util;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.JsonLayout
{
    public struct LayoutColor32Hex
    {
        [JsonIgnore] public Color32 Primivate;

        public string hex
        {
            get => Primivate.ToHexColorString(false, Primivate.a != 0xFF);
            set => Primivate = ColorUtil.ToColor32(value.AsSpan(), 0xFF);
        }

        public byte r
        {
            set => Primivate.r = value;
        }

        public byte g
        {
            set => Primivate.g = value;
        }

        public byte b
        {
            set => Primivate.b = value;
        }

        public byte a
        {
            set => Primivate.a = value;
        }
    }
}