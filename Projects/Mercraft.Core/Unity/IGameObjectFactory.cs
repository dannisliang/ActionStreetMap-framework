
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
        IGameObject CreateNew(string name, IGameObject parent);
        IGameObject CreatePrimitive(string name, UnityPrimitiveType type);

        ISceneVisitor CreateVisitor(IEnumerable<IModelBuilder> builders, 
            IEnumerable<IModelBehaviour> behaviours);
    }
}