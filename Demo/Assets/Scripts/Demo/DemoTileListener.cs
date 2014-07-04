using System;
using System.Diagnostics;
using Mercraft.Core;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Assets.Scripts.Demo
{
    public class DemoTileListener: ITileListener
    {
        private const string LogTag = "Tile";
        [Dependency]
        private ITrace Trace { get; set; }

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public void OnTileLoadStarted(MapPoint center, GeoCoordinate relativeNullPoint)
        {
            _stopwatch.Start();
            Trace.Warn(LogTag, String.Format("Tile loading begin: center:{0}, geo:{1}",
                center, relativeNullPoint));
        }

        public void OnTileLoadFinished(Tile tile)
        {
            _stopwatch.Stop();
            Trace.Normal(LogTag, String.Format("Tile of size {0} is loaded in {1} ms", 
                tile.Size, _stopwatch.ElapsedMilliseconds));
        }

        public void OnTileFound(Tile tile, MapPoint position)
        {
            Trace.Normal(LogTag,
               String.Format("Position {0} is found in tile with center {1}", position, tile.TileMapCenter));
        }
    }
}
