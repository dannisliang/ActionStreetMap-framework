using Mercraft.Core.World.Infos;

namespace Mercraft.Models.Infos
{
    /// <summary>
    ///     Defines info style provider.
    /// </summary>
    public interface IInfoStyleProvider
    {
        /// <summary>
        ///     Gets style for given info.
        /// </summary>
        /// <param name="info">Info.</param>
        /// <returns>Style.</returns>
        InfoStyle Get(Info info);
    }
}
