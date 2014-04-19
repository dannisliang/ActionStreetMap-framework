namespace Mercraft.Models.Buildings.Entities
{
    /// <summary>
    /// This class contains the design constriants for a specific bay that will feature in a facade
    /// </summary>
    public class Bay
    {
        public string Name = "Bay";
        public bool IsOpening = true;
        public float OpeningWidth = 1.25f;
        public float OpeningHeight = 0.85f;
        public float Spacing = 0.5f;

        /// <summary>
        /// Ratio of space between the left and right walls from the opening
        /// </summary>
        public float OpeningWidthRatio = 0.5f;

        /// <summary>
        /// Ratio of space between above and below the opening
        /// </summary>
        public float OpeningHeightRatio = 0.95f;
        public float OpeningDepth = 0.1f;
        public float ColumnDepth = 0.0f;
        public float RowDepth = 0.0f;
        public float CrossDepth = 0.0f;
        public int[] TextureValues = new[] { 1, 0, 0, 0, 0, 0, 0, 0 };   

        public Bay(string name)
        {
            Name = name;
        }

   
        public enum BayTextureName
        {
            OpeningBack,
            OpeningSide,
            OpeningSill,
            OpeningCeiling,
            Column,
            Row,
            Cross,
            Wall
        }

        public int GetTexture(BayTextureName bayTextureName)
        {
            return TextureValues[(int)bayTextureName];
        }

        public void SetTexture(BayTextureName bayTextureName, int textureIndex)
        {
            TextureValues[(int)bayTextureName] = textureIndex;
        }

        public bool IsFlipped(BayTextureName bayTextureName)
        {
            return false;
        }
    }
}
