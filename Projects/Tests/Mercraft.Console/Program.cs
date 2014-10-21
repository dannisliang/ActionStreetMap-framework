using Mercraft.Console.Listeners;
using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Infrastructure.Dependencies;
using Mercraft.Maps.UnitTests;

namespace Mercraft.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var messageBus = new MessageBus();
            var logger = new PerformanceLogger();
            var tileListener = new DemoTileListener(messageBus, logger);

            logger.Start();
            var container = new Container();
            var componentRoot = TestHelper.GetGameRunner(container, messageBus);
            //componentRoot.RunGame(new GeoCoordinate(40.7702587, -73.9827844));
            componentRoot.RunGame(new GeoCoordinate(52.52985, 13.38825));
            var tileManager = container.Resolve<IPositionListener>() as TileManager;

            for (int i = 0; i < 100; i++)
            {
                tileManager.OnMapPositionChanged(new MapPoint(i*10 + 0.1f, 0.1f));
            }

            logger.Stop();
            System.Console.WriteLine("Tiles: {0}", tileManager.Count);
        }
    }
}