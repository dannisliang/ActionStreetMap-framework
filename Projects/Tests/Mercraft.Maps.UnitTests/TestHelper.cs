
using System;
using Mercraft.Core;
using Mercraft.Infrastructure.Config;

namespace Mercraft.Maps.UnitTests
{
    internal static class TestHelper
    {
        public static readonly GeoCoordinate BerlinGeoCenter = new GeoCoordinate(52.529814, 13.388015);
        public static readonly GeoCoordinate BerlinInvalidenStr = new GeoCoordinate(52.531036, 13.384866);

        public const string ConfigAppRootFile = @"..\..\..\..\..\Demo\Config\app.config";
        public const string ConfigTestRootFile = "test.config";

        public const string TestPbfFilePath = @"..\..\..\..\Tests\TestAssets\Osm\kempen.osm.pbf";

        //52.53057 13.38687 52.52940 13.39022
        public const string TestXmlFilePath = @"..\..\..\..\Tests\TestAssets\Osm\berlin_house.osm.xml";

        public const string TestBigPbfFilePath = @"..\..\..\..\..\Demo\Maps\berlin-latest.osm.pbf";

        public const string TestBigPbfIndexListPath = @"..\..\..\..\..\Demo\Maps";

        public const string TestThemeFile = @"..\..\..\..\Tests\TestAssets\Themes\base.xml";
        public const string TestBaseMapcssFile = @"..\..\..\..\Tests\TestAssets\Mapcss\base.mapcss";
        public const string DefaultMapcssFile = @"..\..\..\..\..\Demo\Config\themes\default\default.mapcss";

        public static IPathResolver GetPathResolver()
        {
            return new TestPathResolver();
        }
    }
}
