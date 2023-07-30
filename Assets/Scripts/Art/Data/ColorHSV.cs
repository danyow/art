using UnityEngine;

namespace Art.Data
{
    public struct ColorHSV
    {
        public readonly float h;
        public readonly float s;
        public readonly float v;

        public ColorHSV(Color32 clr)
        {
            Color.RGBToHSV(clr, out h, out s, out v);
        }
    }
}