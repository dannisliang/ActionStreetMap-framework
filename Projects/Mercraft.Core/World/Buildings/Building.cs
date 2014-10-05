using System.Collections.Generic;
using Mercraft.Core.Unity;

namespace Mercraft.Core.World.Buildings
{
    public class Building
    {
        public long Id { get; set; }

        public Address Address { get; set; }
        public IGameObject GameObject { get; set; }
        public float Elevation { get; set; }
        public List<MapPoint> Footprint { get; set; }

        // NOTE OSM-available info (see details http://wiki.openstreetmap.org/wiki/Buildings)

        public string Type { get; set; }

        #region Height specific

        public float Height { get; set; }
        public float MinHeight { get; set; }
        public int Levels { get; set; }

        #endregion

        #region Appearance

        public Color32 FacadeColor { get; set; }
        public string FacadeMaterial { get; set; }

        public Color32 RoofColor { get; set; }
        public string RoofMaterial { get; set; }
        public string RoofType { get; set; }

        #endregion

        #region Characteristics

        /// <summary>
        ///     Indicates that the building is used as a specific shop
        /// </summary>
        public string Shop { get; set; }

        /// <summary>
        ///     Describes what the building is used for, for example: school, theatre, bank
        /// </summary>
        public string Amenity { get; set; }

        /// <summary>
        ///      Ruins of buildings
        /// </summary>
        public string Ruins { get; set; }

        /// <summary>
        ///     For a building which has been abandoned by its owner and is no longer maintained
        /// </summary>
        public string Abandoned { get; set; }

        #endregion
    }
}
