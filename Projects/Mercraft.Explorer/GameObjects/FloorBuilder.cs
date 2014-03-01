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
            floor.transform.position = new Vector3(500, 30, 500);
            floor.transform.localScale = new Vector3(10, 1, 10);

            return floor;
        }
    }
}
