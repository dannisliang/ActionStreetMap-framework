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
        [Dependency]
        private ITrace Trace { get; set; }

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public void OnTileLoadStarted(Vector2 center, GeoCoordinate relativeNullPoint)
        {
            _stopwatch.Start();
            Trace.Normal("Tile", String.Format("Tile loading begin: center:{0}, geo:{1}",
                center, relativeNullPoint));
        }

        public void OnTileLoadFinished(Tile tile)
        {
            _stopwatch.Stop();
            Trace.Normal("Tile", String.Format("Tile of size {0} is loaded in {1} ms", 
                tile.Size, _stopwatch.ElapsedMilliseconds));
        }
    }
}
