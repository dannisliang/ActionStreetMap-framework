using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    /// <summary>
    /// This class contains the design values for a specific roof design
    /// </summary>

    public class RoofDesign
    {

        public RoofDesign(string newName)
        {
            name = newName;
        }

        public string name = "";

        public enum styles
        {
            flat,
            mansard,
            barrel,
            gabled,
            hipped,
            leanto,
            steepled,
            sawtooth//factory
        }

        public enum parapetStyles
        {
            basic,
            fancy,
        }

        //roof
        public styles style = styles.flat;
        public float height = 2.0f;
        public float depth = 1.0f;//used for mansard roofs
        public float floorDepth = 1.0f;//used for mansard roofs
        public int direction = 0;//used for placing ridges
        public int sawtoothTeeth = 4;
        public int barrelSegments = 20;

        //parapet
        public bool parapet = true;//small wall extending the facade somewhat
        public parapetStyles parapetStyle = parapetStyles.basic;
        public Mesh parapetDesign;
        public float parapetDesignWidth = 1.0f;
        public float parapetHeight = 0.25f;
        public float parapetFrontDepth = 0.1f;
        public float parapetBackDepth = 0.2f;

        //dormer
        public bool hasDormers = false;
        [SerializeField]
        private float _dormerWidth = 1.25f;
        [SerializeField]
        private float _dormerHeight = 0.85f;
        [SerializeField]
        private float _dormerRoofHeight = 0.25f;
        [SerializeField]
        private float _minimumDormerSpacing = 0.5f;
        [SerializeField]
        private float _dormerHeightRatio = 0.95f;

        //textures
        public int numberOfTextures
        {
            get { return System.Enum.GetValues(typeof(textureNames)).Length; }
        }

        public enum textureNames
        {
            floor,
            floorB,
            tiles,
            parapet,
            gable,
            window,
            wall,
            dormerRoof
        }

        [SerializeField]
        private int[] _textureValues;
        [SerializeField]
        private bool[] _flipValues;
        public int wallTexture = 0;//this is use then there are no windows

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
                        _textureValues = new int[8] { 2, 2, 2, 0, 0, 1, 0, 2 };
                    }
                }

                return _textureValues;
            }
            set
            {
                _textureValues = textureValues;
            }
        }

        public int GetTexture(textureNames name)
        {
            return textureValues[(int)name];
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
                _textureValues = textureValues;
            }
        }

        public float dormerWidth
        {
            get { return _dormerWidth; }
            set
            {
                _dormerWidth = Mathf.Max(value, 0);
            }
        }

        public float dormerHeight
        {
            get { return _dormerHeight; }
            set
            {
                _dormerHeight = Mathf.Clamp(value, 0, height);
            }
        }

        public float dormerRoofHeight
        {
            get { return _dormerRoofHeight; }
            set
            {
                _dormerRoofHeight = Mathf.Clamp(value, 0, dormerHeight);
            }
        }

        public float minimumDormerSpacing
        {
            get { return _minimumDormerSpacing; }
            set
            {
                _minimumDormerSpacing = Mathf.Max(value, 0);
            }
        }

        public float dormerHeightRatio { get { return _dormerHeightRatio; } set { _dormerHeightRatio = Mathf.Clamp01(value); } }

        public bool IsFlipped(textureNames name)
        {
            return flipValues[(int)name];
        }
    }
}
