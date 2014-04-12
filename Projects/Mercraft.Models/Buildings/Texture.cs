using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public class Texture
    {
        public string name = "new texture";
        public bool tiled = true;
        public bool patterned = false;
        //[SerializeField]
        //private Texture2D _texture;
        [SerializeField] private Vector2 _tileUnitUV = Vector2.one;
            //the UV coords of the end of a pattern in the texture - used to match up textures to geometry

        [SerializeField] private Vector2 _textureUnitSize = Vector2.one;
            //the world size of the texture - default 1m x 1m

        public int tiledX = 1; //the amount of times the texture should be repeated along the x axis
        public int tiledY = 1; //the amount of times the texture should be repeated along the y axis
        public Vector2 maxUVTile = Vector2.zero; //used for texture atlasing
        public Vector2 minWorldUnits = Vector2.zero; //also used for atlasing
        public Vector2 maxWorldUnits = Vector2.zero; //also used for atlasing
        public Material material;
        public Texture2D tiledTexture = null; //this is used for texture packing

        public bool door = false;
        public bool window = false;
        public bool wall = false;
        public bool roof = false;

        public Texture(string newName)
        {
            name = newName;
            material = new Material(Shader.Find("Diffuse"));
        }


        public Texture Duplicate()
        {
            return Duplicate(name + " copy");
        }

        public Texture Duplicate(string newName)
        {
            return new Texture(newName)
            {
                tiledTexture = tiledTexture,
                tiled = true,
                patterned = false,
                tileUnitUV = _tileUnitUV,
                textureUnitSize = _textureUnitSize,
                tiledX = tiledX,
                tiledY = tiledY,
                maxUVTile = maxUVTile,
                material = new Material(material),
                door = door,
                window = window,
                wall = wall,
                roof = roof
            };
        }

        public Texture2D texture
        {
            get
            {
                if (material.mainTexture == null)
                    return null;
                return (Texture2D) material.mainTexture;
            }

            set
            {
                if (value == null)
                    return;
                material.mainTexture = value;
            }
        }

        public Vector2 tileUnitUV
        {
            get { return _tileUnitUV; }
            set { _tileUnitUV = value; }
        }

        public Vector2 textureUnitSize
        {
            get { return _textureUnitSize; }
            set { _textureUnitSize = value; }
        }

        public void CheckMaxUV(Vector2 checkUV)
        {
            if (checkUV.x > maxUVTile.x)
            {
                maxUVTile.x = checkUV.x;
            }
            if (checkUV.y > maxUVTile.y)
            {
                maxUVTile.y = checkUV.y;
            }
        }

        public void MaxWorldUnitsFromUVs(Vector2 uv)
        {
            float xsize = uv.x*_textureUnitSize.x;
            float ysize = uv.y*_textureUnitSize.y;
            if (xsize > maxWorldUnits.x)
            {
                maxWorldUnits.x = xsize;
            }
            if (ysize > maxWorldUnits.y)
            {
                maxWorldUnits.y = ysize;
            }
        }
    }
}
