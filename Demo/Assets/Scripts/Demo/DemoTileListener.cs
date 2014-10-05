using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Mercraft.Core;
using Mercraft.Core.Scene.Models;
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

            //messageBus.AsObservable<TileFoundMessage>().Do(m => OnTileFound(m.Tile, m.Position)).Subscribe();
            messageBus.AsObservable<TileBuildStartMessage>().Do(m => OnTileBuildStarted(m.TileCenter)).Subscribe();
            messageBus.AsObservable<TileBuildFinishMessage>().Do(m => OnTileBuildFinished(m.Tile)).Subscribe();
        }

        public void OnTileFound(Tile tile, MapPoint position)
        {
            //_trace.Normal(LogTag, String.Format("Position {0} is found in tile with center {1}", position, tile.MapCenter));
        }

        public void OnTileBuildStarted(MapPoint center)
        {
            _stopwatch.Start();
            _trace.Normal(LogTag, String.Format("Tile build begin: center:{0}", center));
        }

        public void OnTileBuildFinished(Tile tile)
        {
            _stopwatch.Stop();
            _trace.Normal(LogTag, String.Format("Tile of size {0} is built in {1} ms",
                tile.Size, _stopwatch.ElapsedMilliseconds));
            GC.Collect();
            _stopwatch.Reset();

        }
    }
}
