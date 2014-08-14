using System;
using Assets.Scripts.Map;
using Mercraft.Core.Scene;
using Mercraft.Explorer.Interactions;
using Mercraft.Infrastructure.Bootstrap;

namespace Assets.Scripts.Character
{
    public class DemoBootstrapper: BootstrapperPlugin
    {
        public override string Name
        {
            get { return "demo"; }
        }

        public override bool Run()
        {
            Container.RegisterInstance<IModelBehaviour>(new CompositeModelBehaviour("solid", new Type[]
            {
                typeof(DestroyableObject),
                typeof(LocationInfoHolder),
            }));

            return true;
        }
    }
}
