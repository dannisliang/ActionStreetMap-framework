using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mercraft.Models.Buildings.Utils
{
    //This function creates textures for the packed model
    //It will tile textures for as long as needed to ensure we use one large texture for the model.
    public class BuildingTexturePacker2 : MonoBehaviour
    {

        private static int MAX_SOURCE_TEXTURE_SIZE = 4096;
        private static int ATLAS_PADDING = 0;
        private static int MAXIMUM_TEXTURESIZE = 4096;

        private static float timestart;

        /// <summary>
        /// Custom BuildR texture packer. Uses the BuildR Data class to create a single texture used by the generated building, taking into account tiling.
        /// </summary>
        /// <param name="data">BuildR Building Data</param>
        public static void Pack(Data data)
        {
            timestart = Time.realtimeSinceStartup;
            data.TexturesNotPacked.Clear();
            data.TexturesPacked.Clear();

            int numberOfTextures = data.Textures.Count;
            List<Rect> packedTexturePositions = new List<Rect>();
            List<int> texturePacked = new List<int>();
            for (int t = 0; t < numberOfTextures; t++)
            {
                Texture texture = data.Textures[t];
                if (texture.tiled)
                {
                    int textureWidth = texture.texture.width;
                    int textureHeight = texture.texture.height;
                    int targetTextureWidth = Mathf.RoundToInt(textureWidth * texture.maxUVTile.x);
                    int targetTextureHeight = Mathf.RoundToInt(textureHeight * texture.maxUVTile.y);
                    float resizeToLargest = Mathf.Min((float)MAX_SOURCE_TEXTURE_SIZE / (float)targetTextureWidth, (float)MAX_SOURCE_TEXTURE_SIZE / (float)targetTextureHeight, 1);//ensure texture fits smallest size
                    int useTextureWidth = Mathf.RoundToInt(resizeToLargest * targetTextureWidth);
                    int useTextureHeight = Mathf.RoundToInt(resizeToLargest * targetTextureHeight);

                    if (useTextureWidth == 0 || useTextureHeight == 0)//texture no used on model
                    {
                        data.TexturesNotPacked.Add(t);
                        continue;
                    }
                    packedTexturePositions.Add(new Rect(0, 0, useTextureWidth, useTextureHeight));
                    texturePacked.Add(t);
                }
                else
                {
                    int useTextureWidth = texture.texture.width;
                    int useTextureHeight = texture.texture.height;
                    packedTexturePositions.Add(new Rect(0, 0, useTextureWidth, useTextureHeight));
                    texturePacked.Add(t);
                }
            }

            int atlasedTextureWidth = RectanglePack.Pack(packedTexturePositions, ATLAS_PADDING);

            //determine the resize scale and apply that to the rects
            float packedScale = 1;
            int numberOfRects = packedTexturePositions.Count;
            if (atlasedTextureWidth > MAXIMUM_TEXTURESIZE)
            {
                packedScale = MAXIMUM_TEXTURESIZE / (float)atlasedTextureWidth;
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
                Texture texture = data.Textures[t];
                Rect packedPosition = packedTexturePositions[t];
                int targetTextureWidth = (int)packedPosition.width;
                int targetTextureHeight = (int)packedPosition.height;
                int sourceTextureWidth = texture.texture.width;
                int sourceTextureHeight = texture.texture.height;
                //            int sourceTextureSize = sourceTextureWidth * sourceTextureHeight;
                int paintTextureWidth = Mathf.RoundToInt(targetTextureWidth / (texture.tiled ? texture.maxUVTile.x : texture.tiledX));
                int paintTextureHeight = Mathf.RoundToInt(targetTextureHeight / (texture.tiled ? texture.maxUVTile.y : texture.tiledY));
                int paintTextureSize = paintTextureWidth * paintTextureHeight;
                Color32[] paintColourArray = TextureScale.NearestNeighbourSample(texture.texture.GetPixels32(), sourceTextureWidth, sourceTextureHeight, paintTextureWidth, paintTextureHeight);

                for (int x = 0; x < targetTextureWidth; x++)
                {
                    for (int y = 0; y < targetTextureHeight; y++)
                    {
                        int drawX = Mathf.FloorToInt(x + packedPosition.x);
                        int drawY = Mathf.FloorToInt(y + packedPosition.y);
                        int colourIndex = drawX + drawY * atlasedTextureWidth;

                        int sx = x % paintTextureWidth;
                        int sy = y % paintTextureHeight;
                        int paintIndex = sx + sy * paintTextureWidth;
                        if (paintIndex >= paintTextureSize)
                            Debug.Log("Source Index too big " + sx + " " + sy + " " + paintTextureWidth + " " + paintTextureSize + " " + texture.maxUVTile + " " + texture.name);
                        Color32 sourceColour = paintColourArray[paintIndex];
                        if (colourIndex >= textureSize)
                            Debug.Log("Output Index Too big " + drawX + " " + drawY + " " + colourIndex + " " + textureSize + " " + packedPosition);
                        colourArray[colourIndex] = sourceColour;
                    }
                }
            }

            Texture2D packedTexture = new Texture2D(atlasedTextureWidth, atlasedTextureWidth, TextureFormat.ARGB32, true);
            packedTexture.filterMode = FilterMode.Bilinear;
            packedTexture.SetPixels32(colourArray);
            packedTexture.Apply(true, false);

            //remove textures from memory
            if (data.TextureAtlas != null)
                DestroyImmediate(data.TextureAtlas);

            data.TextureAtlas = packedTexture;
            data.TexturesPacked = texturePacked;
            data.AtlasCoords = RectanglePack.ConvertToUVSpace(packedTexturePositions.ToArray(), atlasedTextureWidth);

            System.GC.Collect();

            //Debug.Log("BuildR Texutre Pack Complete: " + (Time.realtimeSinceStartup - timestart)+" sec");
        }

        public static Rect[] Pack(out Texture2D output, Texture2D[] data)
        {
            return Pack(out output, data, MAXIMUM_TEXTURESIZE);
        }

        /// <summary>
        /// Packs an array of Texture2Ds into a sinlgle Texture2D with a maximum size specified.
        /// </summary>
        /// <param name="output">The output Texture2D</param>
        /// <param name="data">And array of input Texture2Ds</param>
        /// <param name="maximumSize">The maximum size of the output texture</param>
        /// <returns></returns>
        public static Rect[] Pack(out Texture2D output, Texture2D[] data, int maximumSize)
        {
            timestart = Time.realtimeSinceStartup;

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

            int atlasedTextureWidth = RectanglePack.Pack(packedTexturePositions, ATLAS_PADDING);

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
