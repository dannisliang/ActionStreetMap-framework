using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    /// <summary>
    /// This class contains the design constriants for a specific bay that will feature in a facade
    /// </summary>
    public class Bay
    {
        public string name = "Bay design";
        public bool isOpening = true;
        public float openingWidth = 1.25f;
        public float openingHeight = 0.85f;
        public float minimumBayWidth = 0.5f;
        public float openingWidthRatio = 0.5f;//the ratio of space between the left and right walls from the opening
        public float openingHeightRatio = 0.95f;//the ratio of space between above and below the opening
        public float openingDepth = 0.1f;
        public float columnDepth = 0.0f;
        public float rowDepth = 0.0f;
        public float crossDepth = 0.0f;

        public Bay(string newName)
        {
            name = newName;
        }

        //textures
        public int numberOfTextures
        {
            get { return System.Enum.GetValues(typeof(TextureNames)).Length; }
        }

        public enum TextureNames
        {
            OpeningBackTexture,
            OpeningSideTexture,
            OpeningSillTexture,
            OpeningCeilingTexture,
            ColumnTexture,
            RowTexture,
            CrossTexture,
            WallTexture
        }

        private int[] _textureValues;
        private bool[] _flipValues;
        public int wallTexture = 0;//this is use then there are no windows

        public Bay Duplicate()
        {
            Bay newBay = new Bay(name + " copy");
            newBay.isOpening = isOpening;
            newBay.openingWidth = openingWidth;
            newBay.openingHeight = openingHeight;
            newBay.minimumBayWidth = minimumBayWidth;
            newBay.openingWidthRatio = openingWidthRatio;//the ratio of space between the left and right walls from the opening
            newBay.openingHeightRatio = openingHeightRatio;//the ratio of space between above and below the opening
            newBay.openingDepth = openingDepth;
            newBay.columnDepth = columnDepth;
            newBay.rowDepth = rowDepth;
            newBay.crossDepth = crossDepth;
            newBay.textureValues = (int[])textureValues.Clone();
            newBay.flipValues = (bool[])flipValues.Clone();

            return newBay;
        }

        public int[] textureValues
        {
            get
            {
                if (_textureValues == null)
                    _textureValues = new int[0];

                if (_textureValues.Length != numberOfTextures)
                {
                    int[] tempArr = (int[])_textureValues.Clone();
                    int oldSize = tempArr.Length;
                    _textureValues = new int[numberOfTextures];
                    if (oldSize > 0)
                    {
                        for (int i = 0; i < oldSize; i++)
                        {
                            _textureValues[i] = tempArr[i];
                        }
                    }
                    else
                    {
                        _textureValues = new[] { 1, 0, 0, 0, 0, 0, 0, 0 };
                    }
                }

                return _textureValues;
            }
            set
            {
                _textureValues = value;
            }
        }

        public bool[] flipValues
        {
            get
            {
                if (_flipValues == null)
                    _flipValues = new bool[0];

                if (_flipValues.Length != numberOfTextures)
                {
                    bool[] tempArr = (bool[])_flipValues.Clone();
                    int oldSize = tempArr.Length;
                    _flipValues = new bool[numberOfTextures];
                    if (oldSize > 0)
                    {
                        for (int i = 0; i < oldSize; i++)
                        {
                            _flipValues[i] = tempArr[i];
                        }
                    }
                    else
                    {
                        _flipValues = new bool[8];
                    }
                }

                return _flipValues;
            }
            set
            {
                _flipValues = value;
            }
        }

        public int GetTexture(TextureNames textureName)
        {
            return textureValues[(int)textureName];
        }

        public void SetTexture(TextureNames textureName, int textureIndex)
        {
            textureValues[(int)textureName] = textureIndex;
        }

        public bool IsFlipped(TextureNames textureName)
        {
            return flipValues[(int)textureName];
        }
    }
}
