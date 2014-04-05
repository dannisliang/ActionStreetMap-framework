using System;
using System.Linq;
using System.Text;
using Assets.Scripts.Console.Grep;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene;
using Mercraft.Core.Zones;
using Mercraft.Infrastructure.Dependencies;

namespace Assets.Scripts.Console.Commands
{
    public class SceneCommand: ICommand
    {
        private readonly IContainer _container;
        public string Description { get { return "Scene details"; } }

        public SceneCommand(IContainer container)
        {
            _container = container;
        }

        public string Execute(params string[] args)
        {
            var response = new StringBuilder();
            try
            {
                var arguments = new Arguments(args);
                // NOTE assume that we're using default implementation where
                // ZoneLoader is used as IPositionListener 
                var zoneLoader = _container.Resolve<IPositionListener>() as ZoneLoader;
                var tile = zoneLoader.CurrentZone.Tile;
                var currentGeoPosition = GeoProjection.ToGeoCoordinate(zoneLoader.RelativeNullPoint, zoneLoader.CurrentPosition);

                if (arguments["f"] != null)
                    FindModel(tile.Scene, long.Parse((string)arguments["f"]), response);
                else if (arguments["d"] != null)
                {
                    var tileSize = int.Parse((string)arguments["a"]);
                    DumpScene(_container.Resolve<ISceneBuilder>(), currentGeoPosition, tileSize, response);
                }
                else if(arguments["i"] != null)
                    GeneralInfo(zoneLoader, currentGeoPosition, response);
                else
                    PrintHelp(response);
            }
            catch (Exception ex)
            {
                response.AppendFormat("Unable to execute command: {0}", ex);
            }

            return response.ToString();
        }

        /// <summary>
        /// Finds model by given id
        /// </summary>
        private void FindModel(IScene scene, long id, StringBuilder response)
        {
            foreach (var area in scene.Areas)
            {
                if (area.Id == id)
                {
                    response.AppendFormat("Found area: {0}\n", area);
                    foreach (var point in area.Points)
                    {
                        response.AppendLine(point.ToString());
                    }
                    return;
                }
            }

            foreach (var way in scene.Ways)
            {
                if (way.Id == id)
                {
                    response.AppendFormat("Found way: {0}\n", way);
                    foreach (var point in way.Points)
                    {
                        response.AppendLine(point.ToString());
                    }
                    return;
                }
            }

            response.AppendFormat("Item not found: {0}", id);
        }

        private void DumpScene(ISceneBuilder sceneBuilder, GeoCoordinate coordinate, int size, StringBuilder response)
        {
            var bbox = BoundingBox.CreateBoundingBox(coordinate, size / 2);
            var scene = sceneBuilder.Build(bbox);
            foreach (var area in scene.Areas)
            {
               response.AppendFormat("{0}\n", area);
            }

            foreach (var way in scene.Ways)
            {
               response.AppendFormat("{0}\n", way);
            }
        }

        private void GeneralInfo(ZoneLoader zoneLoader, GeoCoordinate currentGeoPosition, StringBuilder response)
        {
            var tile = zoneLoader.CurrentZone.Tile;
            var scene = tile.Scene;
            response.AppendFormat("Geo position: {0}\n", currentGeoPosition);
            response.AppendFormat("Map position: {0}\n", zoneLoader.CurrentPosition);
            response.AppendFormat("Tile size: {0}\n", tile.Size);
            response.AppendFormat("Areas:{0}\n", scene.Areas.Count());
            response.AppendFormat("Ways:{0}", scene.Ways.Count());
        }

        private void PrintHelp(StringBuilder response)
        {
            response.AppendLine("Usage: scene [/f|/a|/i]");
            response.AppendLine("       scene [/i]");
            response.AppendLine("       scene [/f:<id>]");
            response.AppendLine("       scene [/d:<size>]");
        }
    }
}
