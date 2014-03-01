using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Core.Scene
{
    public interface IFloorBuilder
    {
        GameObject Build(Tile tile);
    }
}
