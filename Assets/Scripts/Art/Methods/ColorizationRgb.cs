using Art.Util;
using UnityEngine;

namespace Art.Methods
{
    public class ColorizationRgb: ColorizationMethod
    {
        protected override float ColorCompare(Color32 a, Color32 b)
        {
            return ColorCalc.RgbDist(a, b);
        }
    }
}
