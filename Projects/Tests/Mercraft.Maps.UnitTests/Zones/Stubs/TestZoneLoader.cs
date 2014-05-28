using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.MapCss;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Core.Zones;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests.Zones.Stubs
{
    public class TestZoneLoader : ZoneLoader
    {
        [Dependency]
        public TestZoneLoader(TileProvider tileProvider, IZoneListener zoneListener,
            IStylesheetProvider stylesheetProvider,  
             IGameObjectBuilder sceneModelVisitor) :
            base(tileProvider, zoneListener, stylesheetProvider, sceneModelVisitor)
        {
        }

        public IList<Zone> ZoneCollection
        {
            get
            {
                return Zones.Values.ToList();
            }
        }
    }
}
