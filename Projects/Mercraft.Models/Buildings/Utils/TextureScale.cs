namespace Mercraft.Models.Buildings.Utils
{
    using UnityEngine;

    public class TextureScale
    {

        /// <summary>
        /// Generates a Color32 array of specified size from a source Color32 array using the nearest neighbour algorithm
        /// </summary>
        /// <param name="pixels">Source Color32 Array</param>
        /// <param name="w1">Source width</param>
        /// <param name="h1">Source height</param>
        /// <param name="w2">Target width</param>
        /// <param name="h2">Target height</param>
        /// <returns>A resized Color32 array</returns>
        public static Color32[] NearestNeighbourSample(Color32[] pixels, int w1, int h1, int w2, int h2)
        {
            Color32[] temp = new Color32[w2 * h2];
            float xRatio = w1 / (float)w2;
            float yRatio = h1 / (float)h2;
            int xSamples = Mathf.Max(1, Mathf.RoundToInt(xRatio));
            int ySamples = Mathf.Max(1, Mathf.RoundToInt(yRatio));
            int totalSamples = xSamples * ySamples;
            float px, py;
            for (int i = 0; i < h2; i++)
            {
                for (int j = 0; j < w2; j++)
                {
                    px = Mathf.Floor(j * xRatio);
                    py = Mathf.Floor(i * yRatio);
                    int rSample = 0;
                    int gSample = 0;
                    int bSample = 0;
                    int aSample = 0;
                    for (int sx = 0; sx < xSamples; sx++)
                    {
                        for (int sy = 0; sy < ySamples; sy++)
                        {
                            int samplePixelIndex = Mathf.Min((int)(((py + sy) * w1) + (sx + px)), pixels.Length - 1);
                            Color32 samplePixel = pixels[samplePixelIndex];
                            rSample += samplePixel.r;
                            gSample += samplePixel.g;
                            bSample += samplePixel.b;
                            aSample += samplePixel.a;
                        }
                    }
                    rSample /= totalSamples;
                    gSample /= totalSamples;
                    bSample /= totalSamples;
                    aSample /= totalSamples;
                    int outputIndex = (i * w2) + j;
                    temp[outputIndex].r = (byte)rSample;
                    temp[outputIndex].g = (byte)gSample;
                    temp[outputIndex].b = (byte)bSample;
                    temp[outputIndex].a = (byte)aSample;
                }
            }
            return temp;
        }
    }
}
