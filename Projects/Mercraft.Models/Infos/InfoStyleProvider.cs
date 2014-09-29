using Mercraft.Core.World.Infos;

namespace Mercraft.Models.Infos
{
    public interface IInfoStyleProvider
    {
        InfoStyle Get(Info info);
    }
}
