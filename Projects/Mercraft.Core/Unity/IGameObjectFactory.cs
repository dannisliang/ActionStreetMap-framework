
using System.Collections.Generic;
using Mercraft.Core.Scene;

namespace Mercraft.Core.Unity
{
    /// <summary>
    ///     Creates GameObjects
    /// </summary>
    public interface IGameObjectFactory
    {
        // TODO add object pool logic?
        IGameObject CreateNew(string name);
        IGameObject CreatePrimitive(string name, UnityPrimitiveType type);

        ISceneVisitor GetBuilder(IEnumerable<IModelBuilder> builders, 
            IEnumerable<IModelBehaviour> behaviours);
    }
}