
using UnityEngine;

namespace Mercraft.Scene.Builders
{
    public interface ISceneObjectBuilder<in T>
    {
        GameObject Build(string name, T instance);
    }
}
