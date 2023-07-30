using Art.Util;
using UnityEngine;

namespace Art.Methods
{
    public class ColorizationHsv: ColorizationMethod
    {
        protected override float ColorCompare(Color32 a, Color32 b)
        {
            return ColorCalc.HsvDist(a, b);
        }
    }
}
