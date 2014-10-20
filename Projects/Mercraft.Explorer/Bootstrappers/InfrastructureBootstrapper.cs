using Mercraft.Core.Unity;
using Mercraft.Explorer.Infrastructure;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using Mercraft.Infrastructure.Utilities;

namespace Mercraft.Explorer.Bootstrappers
{
    /// <summary>
    ///     Register infrastructure classes.
    /// </summary>
    public class InfrastructureBootstrapper: BootstrapperPlugin
    {
        /// <inheritdoc />
        public override string Name { get { return "infrastructure"; } }

        /// <inheritdoc />
        public override bool Run()
        {
            Container.Register(Component.For<ITrace>().Use<UnityConsoleTrace>().Singleton());
            Container.Register(Component.For<IGameObjectFactory>().Use<GameObjectFactory>().Singleton());
            Container.Register(Component.For<IObjectPool>().Use<ObjectPool>().Singleton());
            return true;
        }
    }
}
