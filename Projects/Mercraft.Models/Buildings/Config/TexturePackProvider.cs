
using System.Collections.Generic;
using Mercraft.Infrastructure.Config;
using UnityEngine;
using Texture = Mercraft.Models.Buildings.Entities.Texture;

namespace Mercraft.Models.Buildings.Config
{
    /// <summary>
    /// Provides textures used by building logic
    /// </summary>
    public class TexturePackProvider
    {
        private static readonly char[] FileNameDelimiters = { '\\', '/' };
        
        private readonly Dictionary<string, TexturePack> _themas;

        public TexturePackProvider(IConfigSection config)
        {
            _themas = LoadThemas(config);
        }

        public TexturePack Get(string theme)
        {
            return _themas[theme];
        }

        /// <summary>
        /// Loads themas from config
        /// </summary>
        private Dictionary<string, TexturePack> LoadThemas(IConfigSection config)
        {
            var themas = new Dictionary<string, TexturePack>();

            foreach (var themeConfig in config.GetSections("theme"))
            {
                var themeName = themeConfig.GetString("@name");
                var texturePack = new TexturePack();
                foreach (var textureConfig in themeConfig.GetSections("texture"))
                {
                    string filepath = textureConfig.GetString("filepath");
                    string[] splits = filepath.Split(FileNameDelimiters);
                    
                    var texture = new Texture(splits[splits.Length - 1]);

                    texture.Path = filepath;

                    texture.Tiled = textureConfig.GetBool("tiled");
                    texture.Patterned = textureConfig.GetBool("patterned");
                   
                    texture.TileUnitUV = new Vector2(textureConfig.GetFloat("tileUnitUVX"), textureConfig.GetFloat("tileUnitUVY"));
                    texture.TextureUnitSize = new Vector2(textureConfig.GetFloat("textureUnitSizeX"), textureConfig.GetFloat("textureUnitSizeY"));

                    texture.TiledX = textureConfig.GetInt("tiledX");
                    texture.TiledY = textureConfig.GetInt("tiledY");

                    texture.Door = textureConfig.GetBool("door", false);
                    texture.Window = textureConfig.GetBool("window", false);
                    texture.Wall = textureConfig.GetBool("wall", false);
                    texture.Roof = textureConfig.GetBool("roof", false);

                    if (texture.Wall) texturePack.Wall.Add(texture);
                    if (texture.Window) texturePack.Window.Add(texture);
                    if (texture.Door) texturePack.Door.Add(texture);
                    if (texture.Roof) texturePack.Roof.Add(texture);
                }

                themas.Add(themeName, texturePack);
            }
            return themas;
        }
    }
}
