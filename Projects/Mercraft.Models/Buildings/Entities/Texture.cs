using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    /// <summary>
    /// Holds texture parameters
    /// </summary>
    public class Texture
    {
        public string Name;
        public bool Tiled = true;
        public bool Patterned = false;
        private Material _material;
        
        public bool Door = false;
        public bool Window = false;
        public bool Wall = false;
        public bool Roof = false;
        public bool Shop = false;

        public string Path;

        /// <summary>
        /// Amount of times the texture should be repeated along the x axis
        /// </summary>
        public int TiledX = 1; 

        /// <summary>
        /// Amount of times the texture should be repeated along the y axis
        /// </summary>
        public int TiledY = 1; 

        /// <summary>
        /// Used for texture atlasing
        /// </summary>
        public Vector2 MaxUVTile = Vector2.zero;

        /// <summary>
        /// Used for atlasing
        /// </summary>
        public Vector2 MinWorldUnits = Vector2.zero;

        /// <summary>
        /// Used for atlasing
        /// </summary>
        public Vector2 MaxWorldUnits = Vector2.zero;

        /// <summary>
        /// UV coords of the end of a pattern in the texture - used to match up textures to geometry
        /// </summary>
        public Vector2 TileUnitUV = Vector2.one;

        /// <summary>
        /// World size of the texture - default 1m x 1m
        /// </summary>
        public Vector2 TextureUnitSize = Vector2.one;

        public Texture(string name)
        {
            this.Name = name;
            //Material = new Material(Shader.Find("Diffuse"));
        }

        public Texture Duplicate()
        {
            return Duplicate(Name + " copy");
        }

        public Texture Duplicate(string newName)
        {
            return new Texture(newName)
            {
                Tiled = true,
                Patterned = false,
                TileUnitUV = TileUnitUV,
                TextureUnitSize = TextureUnitSize,
                TiledX = TiledX,
                TiledY = TiledY,
                MaxUVTile = MaxUVTile,
                _material = new Material(_material),
                Door = Door,
                Window = Window,
                Wall = Wall,
                Roof = Roof
            };
        }

        public Texture2D MainTexture
        {
            get
            {
                if (Material.mainTexture == null)
                    return null;
                return (Texture2D)Material.mainTexture;
            }

            set
            {
                if (value == null)
                    return;
                Material.mainTexture = value;
            }
        }

        public Material Material
        {
            get { return _material ?? (_material = new Material(Shader.Find("Diffuse"))); }
            set { _material = value; }
        }

        public void CheckMaxUV(Vector2 checkUV)
        {
            if (checkUV.x > MaxUVTile.x)
            {
                MaxUVTile.x = checkUV.x;
            }
            if (checkUV.y > MaxUVTile.y)
            {
                MaxUVTile.y = checkUV.y;
            }
        }
    }
}
