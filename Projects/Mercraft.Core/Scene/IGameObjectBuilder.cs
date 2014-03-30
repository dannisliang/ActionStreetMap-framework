using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Core.Scene
{
    public interface IGameObjectBuilder
    {
        GameObject FromCanvas(GeoCoordinate center, GameObject parent, Rule rule, Canvas canvas);
        GameObject FromArea(GeoCoordinate center, GameObject parent, Rule rule, Area area);
        GameObject FromWay(GeoCoordinate center, GameObject parent, Rule rule, Way way);
    }
}
