using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    [System.Serializable]
    public class FacadeDesign
    {
        public string name = "";

        public enum types
        {
            simple,
            patterned
        }

        public types type = types.simple;

        public bool hasWindows = true;

        public Bay simpleBay = new Bay("Simple Bay");
        public List<int> bayPattern = new List<int>() { 0 };

        //LEGACY DATA
        //This information was replaced with the Buildr Bay system
        //window dimensions
        public float windowWidth = 1.25f;
        public float windowHeight = 0.85f;
        public float minimumWindowSpacing = 0.5f;
        public bool alternatingSpacing = false;
        public float minimumWindowSpacingAlt = 1.0f;
        public float windowHeightRatio = 0.95f;
        public float windowDepth = 0.1f;
        public int tilesx = 1;
        public int tilesy = 1;

        //wall dimensions
        public float columnDepth = 0.0f;
        public float rowDepth = 0.0f;
        public float crossDepth = 0.0f;

        //door dimensions
        public bool hasDoors = true;
        public float doorWidth = 1.75f;
        public float doorHeight = 2.25f;
        public float doorPosition = 0.5f;
        //textures
        public int numberOfTextures
        {
            get { return System.Enum.GetValues(typeof(textureNames)).Length; }
        }

        public enum textureNames
        {
            columnTexture,
            rowTexture,
            crossTexture,
            doorTexture,
            windowTexture,
            windowBoxTexture,
            windowSillTexture,
            windowCeilingTexture
        }

        [SerializeField]
        private int[] _textureValues;
        [SerializeField]
        private bool[] _flipValues;
        public int wallTexture = 0;//this is use then there are no windows

        public FacadeDesign Duplicate()
        {
            FacadeDesign newDesign = new FacadeDesign();
            newDesign.name = name + " copy";
            newDesign.type = type;
            newDesign.hasWindows = hasWindows;
            newDesign.simpleBay = simpleBay.Duplicate();
            newDesign.bayPattern = new List<int>(bayPattern);

            //Legacy
            newDesign.windowWidth = windowWidth;
            newDesign.windowHeight = windowHeight;
            newDesign.minimumWindowSpacing = minimumWindowSpacing;
            newDesign.alternatingSpacing = alternatingSpacing;
            newDesign.minimumWindowSpacingAlt = minimumWindowSpacingAlt;
            newDesign.windowHeightRatio = windowHeightRatio;
            newDesign.windowDepth = windowDepth;
            newDesign.tilesx = tilesx;
            newDesign.tilesy = tilesy;
            newDesign.columnDepth = columnDepth;
            newDesign.rowDepth = rowDepth;
            newDesign.crossDepth = crossDepth;

            newDesign.hasDoors = hasDoors;
            newDesign.doorWidth = doorWidth;
            newDesign.doorHeight = doorHeight;
            newDesign.doorPosition = doorPosition;
            newDesign.textureValues = (int[])textureValues.Clone();
            newDesign.flipValues = (bool[])flipValues.Clone();
            newDesign.wallTexture = wallTexture;

            return newDesign;
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
                        _textureValues = new int[8] { 0, 0, 0, 0, 1, 0, 0, 0 };
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

        public int GetTexture(textureNames name)
        {
            return textureValues[(int)name];
        }

        public bool IsFlipped(textureNames name)
        {
            return flipValues[(int)name];
        }

        public FacadeDesign(string newName)
        {
            name = newName;
        }

        public FacadeDesign()
        {

        }
    }
}
