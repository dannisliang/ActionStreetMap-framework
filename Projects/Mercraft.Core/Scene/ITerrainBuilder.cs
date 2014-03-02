using Mercraft.Core.Tiles;
using UnityEngine;

namespace Mercraft.Core.Scene
{
    public interface ITerrainBuilder
    {
        GameObject Build(Tile tile);
    }
}
