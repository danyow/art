using Art.Util;
using UnityEngine;

namespace Art.Methods
{
    public class ColorizationHsvWeight: ColorizationMethod
    {
        [SerializeField]
        private float weightH = 1;

        [SerializeField]
        private float weightS = 1;

        [SerializeField]
        private float weightV = 1;

        protected override float ColorCompare(Color32 a, Color32 b)
        {
            return ColorCalc.HsvDist3(a, b, weightH, weightS, weightV);
        }
    }
}
