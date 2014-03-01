using System;
using Mercraft.Infrastructure.Config;

namespace Mercraft.Maps.UnitTests.Infrastructure.Stubs
{
    class ConfigurableClass: IConfigurable
    {
        public IConfigSection ConfigSection { get; set; }
        public void Configure(IConfigSection config)
        {
            ConfigSection = config;
        }
    }
}
