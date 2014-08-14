using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Mercraft.Core;
using Mercraft.Core.Tiles;
using Mercraft.Core.Zones;
using Mercraft.Infrastructure.Diagnostic;

namespace Assets.Scripts.Demo
{
    public class DemoZoneListener
    {
        private const string LogTag = "Zone";
        
        private readonly ITrace _trace;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public DemoZoneListener(IMessageBus messageBus, ITrace trace)
        {
            _trace = trace;

            messageBus.AsObservable<ZoneLoadStartMessage>().Do(m => OnZoneLoadStarted(m.Tile)).Subscribe();
            messageBus.AsObservable<ZoneLoadFinishMessage>().Do(m => OnZoneLoadFinished(m.Zone)).Subscribe();
        }

        public void OnZoneLoadStarted(Tile tile)
        {
            _stopwatch.Start();
            _trace.Normal(LogTag, String.Format("Zone loading begin: {0} {1}", tile.TileMapCenter, tile.RelativeNullPoint));
        }

        public void OnZoneLoadFinished(Zone zone)
        {
            _stopwatch.Stop();
            _trace.Normal(LogTag, String.Format("Zone is loaded in {0} ms", _stopwatch.ElapsedMilliseconds));
            _trace.Normal(LogTag, "Trigger GC");
            System.GC.Collect();
        }
    }
}
