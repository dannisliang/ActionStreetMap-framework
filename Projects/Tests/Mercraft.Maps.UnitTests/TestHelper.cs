
using System;
using Mercraft.Core;
using Mercraft.Explorer;
using Mercraft.Explorer.Bootstrappers;
using Mercraft.Infrastructure.Bootstrap;
using Mercraft.Infrastructure.Config;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.UnitTests.Zones.Stubs;

namespace Mercraft.Maps.UnitTests
{
    public static class TestHelper
    {
        public static readonly GeoCoordinate BerlinGeoCenter = new GeoCoordinate(52.529814, 13.388015);
        public static readonly GeoCoordinate BerlinInvalidenStr = new GeoCoordinate(52.531036, 13.384866);

        public const string ConfigTestRootFile = "test.config";
        public const string ConfigAppRootFile = @"..\..\..\..\..\Demo\Config\app.config";

        public const string TestPbfFilePath = @"..\..\..\..\Tests\TestAssets\Osm\kempen.osm.pbf";

        //52.53057 13.38687 52.52940 13.39022
        public const string TestXmlFilePath = @"..\..\..\..\Tests\TestAssets\Osm\berlin_house.osm.xml";

        public const string TestBigPbfFilePath = @"..\..\..\..\..\Demo\Maps\berlin-latest.osm.pbf";

        public const string TestBigPbfIndexListPath = @"..\..\..\..\..\Demo\Maps";

        public const string TestThemeFile = @"..\..\..\..\Tests\TestAssets\Themes\theme.config";
        public const string TestBaseMapcssFile = @"..\..\..\..\Tests\TestAssets\Mapcss\base.mapcss";
        public const string DefaultMapcssFile = @"..\..\..\..\..\Demo\Config\themes\default\default.mapcss";

        public static IGameRunner GetGameRunner()
        {
            return GetGameRunner(new Container());
        }

        public static IGameRunner GetGameRunner(IContainer container)
        {
            // these items are used during boot process
            var pathResolver = GetPathResolver();
            container.RegisterInstance<IPathResolver>(pathResolver);
            container.RegisterInstance<IConfigSection>(new ConfigSettings(ConfigAppRootFile, pathResolver).GetRoot());

            // actual boot service
            container.Register(Component.For<IBootstrapperService>().Use<BootstrapperService>());

            // boot plugins
            container.Register(Component.For<IBootstrapperPlugin>().Use<InfrastructureBootstrapper>().Named("infrastructure"));
            container.Register(Component.For<IBootstrapperPlugin>().Use<OsmBootstrapper>().Named("osm"));
            container.Register(Component.For<IBootstrapperPlugin>().Use<ZoneBootstrapper>().Named("zone"));
            container.Register(Component.For<IBootstrapperPlugin>().Use<SceneBootstrapper>().Named("scene"));
            container.Register(Component.For<IBootstrapperPlugin>().Use<TestBootstrapperPlugin>().Named("test"));

            return new GameRunner(container);
        }

        public static IPathResolver GetPathResolver()
        {
            return new TestPathResolver();
        }
    }
}
