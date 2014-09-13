using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Mercraft.Core;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Diagnostic;

namespace Assets.Scripts.Demo
{
    public class DemoTileListener
    {
        private const string LogTag = "Tile";

        private readonly ITrace _trace;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public DemoTileListener(IMessageBus messageBus, ITrace trace)
        {
            _trace = trace;

            messageBus.AsObservable<TileLoadStartMessage>().Do(m => OnTileLoadStarted(m.Tile.MapCenter)).Subscribe();
            messageBus.AsObservable<TileLoadFinishMessage>().Do(m => OnTileLoadFinished(m.Tile)).Subscribe();
            messageBus.AsObservable<TileFoundMessage>().Do(m => OnTileFound(m.Tile, m.Position)).Subscribe();
        }

        public void OnTileLoadStarted(MapPoint center)
        {
            _stopwatch.Start();
            _trace.Warn(LogTag, String.Format("Tile loading begin: center:{0}", center));
        }

        public void OnTileLoadFinished(Tile tile)
        {
            _stopwatch.Stop();
            _trace.Normal(LogTag, String.Format("Tile of size {0} is loaded in {1} ms", 
                tile.Size, _stopwatch.ElapsedMilliseconds));
            System.GC.Collect();
        }

        public void OnTileFound(Tile tile, MapPoint position)
        {
            //_trace.Normal(LogTag, String.Format("Position {0} is found in tile with center {1}", position, tile.MapCenter));
        }
    }
}
