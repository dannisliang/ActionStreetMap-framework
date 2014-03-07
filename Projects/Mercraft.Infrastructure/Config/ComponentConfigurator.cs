
using System;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Utilities;

namespace Mercraft.Infrastructure.Config
{
    public class ComponentConfigurator
    {
        [Dependency]
        public IContainer Container { get; set; }

        [Dependency]
        public ConfigSettings Config { get; set; }

        # region RegisterComponent  helpers

        public void RegisterComponent<T>(IConfigSection componentSection)
        {
            RegisterComponent(typeof(T), String.Empty, componentSection);
        }

        public void RegisterComponent(IConfigSection componentSection)
        {
            RegisterComponent(null, String.Empty, componentSection);
        }

        public void RegisterNamedComponent<T>(IConfigSection componentSection)
        {
            RegisterComponent(typeof(T), componentSection.GetString(ConfigKeys.Name), componentSection);
        }

        public void RegisterNamedComponent(IConfigSection componentSection)
        {
            RegisterComponent(null, componentSection.GetString(ConfigKeys.Name), componentSection);
        }

        public void RegisterComponent(Type interfaceType, string name, IConfigSection componentSection)
        {
            var componentType = componentSection.GetType(ConfigKeys.Type);
            var interfaceTypeTmp = interfaceType ?? componentType;

            var component = Component.For(interfaceTypeTmp).Use(componentType, new Type[0])
                .Named(name)
                .Singleton();

            if (typeof(IConfigurable).IsAssignableFrom(componentType))
            {
                var configPath = componentSection.GetString(ConfigKeys.Path);
                component.SetConfig(configPath == null ? componentSection : Config.GetSection(configPath));
            }

            Container.Register(component);
        }

        #endregion
    }
}
