using Mercraft.Core;
using Mercraft.Core.Scene;
using Mercraft.Infrastructure.Dependencies;

namespace Mercraft.Maps.UnitTests
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RunGame();
        }

        public static void RunGame()
        {
            var messageBus = new MessageBus();
            var logger = new PerformanceLogger();
            var tileListener = new DemoTileListener(messageBus, logger);

            logger.Start();
            var container = new Container();
            var componentRoot = TestHelper.GetGameRunner(container, messageBus);
            //componentRoot.RunGame(new GeoCoordinate(40.7702587, -73.9827844));
            //componentRoot.RunGame(new GeoCoordinate(40.7682664, -73.9820302));
            //componentRoot.RunGame(new GeoCoordinate(52.5195675, 13.3621738));

            componentRoot.RunGame(new GeoCoordinate(52.5147205, 13.3510851));

            var tileManager = container.Resolve<IPositionListener>() as TileManager;

            /*for (int i = 0; i < 1000; i++)
            {
                tileManager.OnMapPositionChanged(new MapPoint(-i*10 + 0.1f, 0.1f));
            }*/

            //tileManager.OnMapPositionChanged(new MapPoint(500, 0.1f));

            logger.Stop();
            System.Console.WriteLine("Tiles: {0}", tileManager.Count);
        }
    }
}