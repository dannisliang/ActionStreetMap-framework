using UnityEngine;

namespace Mercraft.Models.Unity
{
    /// <summary>
    ///     Extracts TerrainData behavior
    /// </summary>
    public interface ITerrainData
    {
        int AlphamapResolution { get; set; }
        void SetAlphamaps(int x, int y, float[,,] map);
        Vector3 Size { get; set; }
    }

    /// <summary>
    ///     Wraps unity TerrainData
    /// </summary>
    public class UnityTerrainData : ITerrainData
    {
        private readonly TerrainData _terrainData;

        public UnityTerrainData(TerrainData terrainData)
        {
            _terrainData = terrainData;
        }

        public int AlphamapResolution
        {
            get { return _terrainData.alphamapResolution; }
            set { _terrainData.alphamapResolution = value; }
        }

        public void SetAlphamaps(int x, int y, float[,,] map)
        {
            _terrainData.SetAlphamaps(x, y, map);
        }

        public Vector3 Size
        {
            get { return _terrainData.size; }
            set { _terrainData.size = value; }
        }
    }
}