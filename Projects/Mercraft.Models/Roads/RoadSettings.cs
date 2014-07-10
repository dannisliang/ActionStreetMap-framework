using System.Collections.Generic;
using Mercraft.Core;
using Mercraft.Core.Unity;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadSettings
    {
        public int Width;
        public MapPoint[] Points;
        public IGameObject TargetObject;
        public IGameObject TerrainObject;
    }
}
