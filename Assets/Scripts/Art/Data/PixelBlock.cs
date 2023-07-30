using Art.Methods;
using UnityEngine;

namespace Art.Data
{
    public class PixelBlock
    {
        public Color32[,] pixels{ get; private set; }

        public PixelBlock(Color32[,] pixels)
        {
            this.pixels = pixels;
        }

        public static Color32 ColorAverage(PixelBlock pb, int alphaCut = PixarMethod.kDefaultAlphaCut)
        {
            var colorSum = Vector4.zero;
            float divideSum = 0;
            foreach(var pixel in pb.pixels)
            {
                if(pixel.a > alphaCut)
                {
                    colorSum += new Vector4(pixel.r, pixel.g, pixel.b, pixel.a);
                    divideSum++;
                }
            }

            if(divideSum > 0)
            {
                colorSum /= divideSum;

                return new Color32((byte)colorSum.x, (byte)colorSum.y, (byte)colorSum.z, (byte)colorSum.w);
            }
            return new Color32(1, 1, 1, 0);
        }
    }
}
