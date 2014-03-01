using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Explorer.GameObjects
{
    public class FloorBuilder : IFloorBuilder
    {
        public GameObject Build(Tile tile)
        {
            // TODO calculater real values
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.transform.position = new Vector3(0, 0, 0);
            floor.transform.localScale = new Vector3(1000, 1, 1000);

            return floor;
        }
    }
}
