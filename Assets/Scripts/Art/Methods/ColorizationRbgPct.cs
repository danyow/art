using Art.Util;
using UnityEngine;

namespace Art.Methods
{
    public class ColorizationRbgPct: ColorizationMethod
    {
        protected override float ColorCompare(Color32 a, Color32 b)
        {
            return ColorCalc.RgbDistPerceptive(a, b);
        }
    }
}
