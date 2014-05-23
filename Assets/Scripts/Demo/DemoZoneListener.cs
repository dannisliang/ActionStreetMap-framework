using System;
using System.Diagnostics;
using Mercraft.Core.Tiles;
using Mercraft.Core.Zones;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;

namespace Assets.Scripts.Demo
{
    public class DemoZoneListener: IZoneListener
    {
        [Dependency]
        private ITrace Trace { get; set; }

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public void OnZoneLoadStarted(Tile tile)
        {
            _stopwatch.Start();
            Trace.Normal("Zone", String.Format("Zone loading begin: {0} {1}", tile.TileMapCenter, tile.RelativeNullPoint));
        }

        public void OnZoneLoadFinished(Zone zone)
        {
            _stopwatch.Stop();
            Trace.Normal("Zone", String.Format("Zone is loaded in {0} ms", _stopwatch.ElapsedMilliseconds));
        }
    }
}
