using System;
using ActionStreetMap.Infrastructure.Config;

namespace ActionStreetMap.Tests.Infrastructure.Stubs
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
