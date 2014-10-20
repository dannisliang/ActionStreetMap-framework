using Mercraft.Core.World.Buildings;

namespace Mercraft.Models.Buildings
{
    /// <summary>
    ///     Specifies buildings style provider logic.
    /// </summary>
    public interface IBuildingStyleProvider
    {
        /// <summary>
        ///     Gets building style fpr given building.
        /// </summary>
        /// <param name="building">Building.</param>
        /// <returns>Style.</returns>
        BuildingStyle Get(Building building);
    }
}