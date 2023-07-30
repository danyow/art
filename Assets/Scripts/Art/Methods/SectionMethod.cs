using Art.Data;
using UnityEngine;

namespace Art.Methods
{
    public class SectionMethod: PixarMethod
    {
        [SerializeField]
        private int sectionCountXY;

        [SerializeField]
        private int sectionCountZ;

        public override Texture2D Process(PixelatedIn input)
        {
            var srcTex = input.texture;
            var alphaCut = input.alphaCut;
            var srcPixels = srcTex.GetPixels32();

            for(var loop = 0; loop < srcPixels.Length; loop++)
            {
                if(srcPixels[loop].a < alphaCut)
                {
                    srcPixels[loop] = new Color32(0, 0, 0, 0);
                    continue;
                }

                srcPixels[loop] = RoundOffNormal(srcPixels[loop]);
            }

            srcTex.name = name + "Output";

            srcTex.SetPixels32(srcPixels);
            srcTex.Apply(false);

            return srcTex;
        }

        private Color32 RoundOffNormal(Color32 color)
        {
            var normal = new Vector3(
                color.r / 255f * 2 - 1,
                color.g / 255f * 2 - 1,
                color.b / 255f * 2 - 1
            );

            //xy乞搁 困 / x+ -> y+
            var alphaXY = Mathf.Atan2(normal.y, normal.x); //alpha
            alphaXY = alphaXY < 0 ? alphaXY + Mathf.PI * 2 : alphaXY;

            //z+绵苞狼 阿
            var betaZ = Mathf.Acos(normal.z); //beta
            betaZ = betaZ < 0 ? betaZ + Mathf.PI * 2 : betaZ;

            var dividerXY = Mathf.PI * 2f / sectionCountXY;
            var dividerZ = Mathf.PI / 2 / sectionCountZ;
            var divXY = alphaXY / dividerXY;
            var divZ = betaZ / dividerZ;

            var alphaXYSect = ((int)divXY + .5f) * dividerXY;
            var betaZSect = ((int)divZ + .5f) * dividerZ;

            var outNormal = new Vector3(
                Mathf.Sin(betaZSect) * Mathf.Cos(alphaXYSect),
                Mathf.Sin(betaZSect) * Mathf.Sin(alphaXYSect),
                Mathf.Cos(betaZSect)
            ).normalized;

            return new Color(
                (outNormal.x + 1) * .5f,
                (outNormal.y + 1) * .5f,
                (outNormal.z + 1) * .5f,
                1
            );
        }
    }
}
