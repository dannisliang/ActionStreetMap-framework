using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Models.Unity;
using UnityEngine;

namespace Mercraft.Maps.UnitTests.Models.Stubs
{
    /// <summary>
    /// Use this class instead of mocking as Moq cannot mock multidimensional array
    /// </summary>
    public class TestTerrainData: ITerrainData
    {
        public int AlphamapResolution { get; set; }

        public int X;
        public int Y;
        public float[,,] Map;

        public void SetAlphamaps(int x, int y, float[,,] map)
        {
            X = x;
            Y = y;
            Map = map;
        }

        public Vector3 Size { get; set; }
    }
}
