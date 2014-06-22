using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.MapCss;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Core.Unity;
using Mercraft.Core.Zones;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestZoneLoader : ZoneLoader
    {
        [Dependency]
        public TestZoneLoader(TileProvider tileProvider, IZoneListener zoneListener,
            IStylesheetProvider stylesheetProvider,
            IGameObjectFactory goFactory,
            IEnumerable<IModelBuilder> builders,
            IEnumerable<IModelBehaviour> behaviours) :
                base(tileProvider, zoneListener, stylesheetProvider, goFactory, builders, behaviours)
        {
        }

        public IList<Zone> ZoneCollection
        {
            get { return Zones.Values.ToList(); }
        }
    }
}