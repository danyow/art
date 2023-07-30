using Art.Data;
using UnityEngine;

namespace Art.Methods
{
    public class PixelatedMethod: PixarMethod
    {

        public override Texture2D Process(PixelatedIn input)
        {
            var srcTex = input.texture;
            var blockSize = input.blockSize;

            var inputPixels = srcTex.GetPixels32();

            var wh = new Vector2Int(
                srcTex.width / blockSize + (srcTex.width % blockSize > 0 ? 1 : 0),
                srcTex.height / blockSize + (srcTex.height % blockSize > 0 ? 1 : 0)
            );

            //foreach blocks
            var pBlocks = new PixelBlock[wh.y, wh.x];
            for(var by = 0; by < pBlocks.GetLength(0); by++)
            {
                var blockSizeY = CalcBlockSize(by, blockSize, srcTex.height);

                for(var bx = 0; bx < pBlocks.GetLength(1); bx++)
                {
                    var blockSizeX = CalcBlockSize(bx, blockSize, srcTex.width);

                    var blockPixels = new Color32[blockSizeY, blockSizeX];

                    var samplePointX = bx * blockSize;
                    var samplePointY = by * blockSize;

                    //foreach blocks' pixels
                    for(var py = 0; py < blockPixels.GetLength(0); py++)
                    {
                        for(var px = 0; px < blockPixels.GetLength(1); px++)
                        {
                            var inputPixelIdx = (samplePointY + py) * srcTex.width + (samplePointX + px);
                            blockPixels[py, px] = inputPixels[inputPixelIdx];
                        }
                    }

                    pBlocks[by, bx] = new PixelBlock(blockPixels);
                }
            }

            srcTex.Reinitialize(pBlocks.GetLength(1), pBlocks.GetLength(0), TextureFormat.RGBA32, false);
            srcTex.filterMode = FilterMode.Point;
            srcTex.wrapMode = TextureWrapMode.Clamp;
            srcTex.name = name + "Output";

            for(var y = 0; y < pBlocks.GetLength(0); y++)
            {
                for(var x = 0; x < pBlocks.GetLength(1); x++)
                {
                    srcTex.SetPixel(x, y, ColorAverage(pBlocks[y, x]));
                }
            }

            srcTex.Apply(false);

            return srcTex;
        }

        private static int CalcBlockSize(int idx, int defaultSize, int pixelLength)
        {
            var size = defaultSize;
            if((idx + 1) * defaultSize > pixelLength)
            {
                size = pixelLength - (idx * defaultSize);
            }

            return size;
        }

        private static Color32 ColorAverage(PixelBlock pb, int alphaCut = kDefaultAlphaCut)
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
