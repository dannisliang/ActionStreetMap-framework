using System;
using Mercraft.Core.Scene.World.Buildings;

namespace Mercraft.Models.Buildings.Roofs
{
    /// <summary>
    ///     Builds gabled roof.
    ///     See http://wiki.openstreetmap.org/wiki/Key:roof:shape#Roof
    /// </summary>
    public class GabledRoofBuilder:IRoofBuilder
    {
        /// <inheritdoc />
        public string Name { get { return "gabled"; } }

        /// <inheritdoc />
        public bool CanBuild(Building building)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public MeshData Build(Building building, BuildingStyle style)
        {
            throw new NotImplementedException();
        }
    }
}
