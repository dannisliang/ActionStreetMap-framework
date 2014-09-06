using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Mercraft.Core;
using Mercraft.Core.Tiles;
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

            messageBus.AsObservable<TileLoadStartMessage>().Do(m => OnTileLoadStarted(m.Tile)).Subscribe();
            messageBus.AsObservable<TileLoadFinishMessage>().Do(m => OnTileLoadFinished(m.Tile)).Subscribe();
        }

        public void OnTileLoadStarted(Tile tile)
        {
            _stopwatch.Start();
            _trace.Normal(LogTag, String.Format("Zone loading begin: {0} {1}", tile.MapCenter, tile.RelativeNullPoint));
        }

        public void OnTileLoadFinished(Tile tile)
        {
            _stopwatch.Stop();
            _trace.Normal(LogTag, String.Format("Zone is loaded in {0} ms", _stopwatch.ElapsedMilliseconds));
            _trace.Normal(LogTag, "Trigger GC");
            System.GC.Collect();
        }
    }
}
