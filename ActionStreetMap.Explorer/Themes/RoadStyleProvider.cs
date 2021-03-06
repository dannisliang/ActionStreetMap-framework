﻿using System.Collections.Generic;
using System.Linq;
using ActionStreetMap.Core.Scene.World.Roads;
using ActionStreetMap.Models.Roads;

namespace ActionStreetMap.Explorer.Themes
{
    /// <summary>
    ///     Default road style provider.
    /// </summary>
    public class RoadStyleProvider : IRoadStyleProvider
    {
        private readonly Dictionary<string, List<RoadStyle>> _roadTypeStyleMapping;

        /// <summary>
        ///     Creates RoadStyleProvider.
        /// </summary>
        /// <param name="roadTypeStyleMapping">Road type to style mapping.</param>
        public RoadStyleProvider(Dictionary<string, List<RoadStyle>> roadTypeStyleMapping)
        {
            _roadTypeStyleMapping = roadTypeStyleMapping;
        }

        /// <inheritdoc />
        public RoadStyle Get(Road road)
        {
            // NOTE use first element's type
            //var type = road.Elements[0].Type;

            // TODO use smart logic to choose road style
            var type = _roadTypeStyleMapping.Keys.First();

            return _roadTypeStyleMapping[type][0];
        }
    }
}
