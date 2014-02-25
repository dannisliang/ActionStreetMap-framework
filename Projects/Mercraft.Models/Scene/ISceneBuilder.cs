using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mercraft.Models.Scene
{
    public interface ISceneBuilder
    {
        IScene Build(GeoCoordinate center, BoundingBox bbox);
    }
}
