using System;
using ActionStreetMap.Infrastructure.Config;

namespace ActionStreetMap.Maps.UnitTests.Infrastructure.Stubs
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
