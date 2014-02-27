
using UnityEngine;

namespace Mercraft.Core.Zones
{
    public interface IGameObjectBuilder
    {
        GameObject Build(string name, object instance);
    }

    public interface IGameObjectBuilder<in T>: IGameObjectBuilder
    {
        GameObject Build(string name, T instance);
    }
}
