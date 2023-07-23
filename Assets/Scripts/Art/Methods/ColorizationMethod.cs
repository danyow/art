using Art.Data;
using UnityEngine;

namespace Art.Methods
{
    public abstract class ColorizationMethod: PixarMethod
    {
        [SerializeField]
        protected Palette palette;

        public override Texture2D Process(PixelatedIn input)
        {
            var srcTex = input.texture;
            var paletteColors = palette.colors;
            var alphaCut = input.alphaCut;

            var srcPixels = srcTex.GetPixels32();

            for(var loop = 0; loop < srcPixels.Length; loop++)
            {
                if(srcPixels[loop].a < alphaCut)
                {
                    srcPixels[loop] = new Color32(0, 0, 0, 0);
                    continue;
                }

                var pixel = srcPixels[loop];

                float closestDistance = int.MaxValue;
                var closeColor = default(Color32);

                for(var loop2 = 0; loop2 < paletteColors.Length; loop2++)
                {
                    var d = ColorCompare(pixel, paletteColors[loop2]);
                    if(d < closestDistance)
                    {
                        closestDistance = d;
                        closeColor = paletteColors[loop2];
                    }
                }

                srcPixels[loop] = closeColor;
            }

            srcTex.name = name + "Output";

            srcTex.SetPixels32(srcPixels);
            srcTex.Apply(false);

            return srcTex;
        }

        protected abstract float ColorCompare(Color32 a, Color32 b);
    }
}
