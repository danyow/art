using Art.Util;
using UnityEngine;

namespace Art.Methods
{
    public class ColorizationHsv2: ColorizationMethod
    {
        protected override float ColorCompare(Color32 a, Color32 b)
        {
            return ColorCalc.HsvDist2(a, b);
        }
    }
}
