using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Models.Buildings.Entities;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Utils
{
    public class BuildingTexturePacker
    {
        private const int AtlasPadding = 0;

        /// <summary>
        /// Packs an array of Texture2Ds into a sinlgle Texture2D with a maximum size specified.
        /// </summary>
        /// <param name="output">The Output Texture2D</param>
        /// <param name="data">And array of input Texture2Ds</param>
        /// <param name="maximumSize">The maximum size of the Output texture</param>
        public static Rect[] Pack(out Texture2D output, Texture2D[] data, int maximumSize)
        {
            int numberOfTextures = data.Length;
            List<Rect> packedTexturePositions = new List<Rect>();
            List<int> texturePacked = new List<int>();
            for (int t = 0; t < numberOfTextures; t++)
            {
                Texture2D texture = data[t];
                int useTextureWidth = texture.width;
                int useTextureHeight = texture.height;
                packedTexturePositions.Add(new Rect(0, 0, useTextureWidth, useTextureHeight));
                texturePacked.Add(t);
            }

            int atlasedTextureWidth = RectanglePack.Pack(packedTexturePositions, AtlasPadding);

            //determine the resize scale and apply that to the rects
            int numberOfRects = packedTexturePositions.Count;
            if (atlasedTextureWidth > maximumSize)
            {
                float packedScale = maximumSize / (float)atlasedTextureWidth;
                for (int i = 0; i < numberOfRects; i++)
                {
                    Rect thisRect = packedTexturePositions[i];
                    thisRect.x *= packedScale;
                    thisRect.y *= packedScale;
                    thisRect.width *= packedScale;
                    thisRect.height *= packedScale;
                    packedTexturePositions[i] = thisRect;
                }
                atlasedTextureWidth = Mathf.RoundToInt(packedScale * atlasedTextureWidth);
            }
            else
            {
                atlasedTextureWidth = (int)Mathf.Pow(2, (Mathf.FloorToInt(Mathf.Log(atlasedTextureWidth - 1, 2)) + 1));//find the next power of two
            }

            int textureSize = atlasedTextureWidth * atlasedTextureWidth;
            Color32[] colourArray = new Color32[textureSize];

            //Build colour array
            for (int t = 0; t < numberOfTextures; t++)
            {
                Texture2D texture = data[t];
                Rect packedPosition = packedTexturePositions[t];
                int targetTextureWidth = (int)packedPosition.width;
                int targetTextureHeight = (int)packedPosition.height;
                int sourceTextureWidth = texture.width;
                int sourceTextureHeight = texture.height;
                Color32[] paintColourArray = TextureScale.NearestNeighbourSample(texture.GetPixels32(), sourceTextureWidth, sourceTextureHeight, targetTextureWidth, targetTextureHeight);

                for (int x = 0; x < targetTextureWidth; x++)
                {
                    for (int y = 0; y < targetTextureHeight; y++)
                    {
                        int drawX = Mathf.FloorToInt(x + packedPosition.x);
                        int drawY = Mathf.FloorToInt(y + packedPosition.y);
                        int colourIndex = drawX + drawY * atlasedTextureWidth;
                        int paintIndex = x + y * targetTextureWidth;
                        Color32 sourceColour = paintColourArray[paintIndex];
                        colourArray[colourIndex] = sourceColour;
                    }
                }
            }

            output = new Texture2D(atlasedTextureWidth, atlasedTextureWidth, TextureFormat.ARGB32, true);
            output.filterMode = FilterMode.Bilinear;
            output.SetPixels32(colourArray);
            output.Apply(true, false);

            //Debug.Log("BuildR Texutre Pack Complete: " + (Time.realtimeSinceStartup - timestart) + " sec");

            return RectanglePack.ConvertToUVSpace(packedTexturePositions.ToArray(), atlasedTextureWidth);
        }
    }
}
