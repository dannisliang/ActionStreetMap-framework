using System;
using System.Linq;
using System.Text;
using Assets.Scripts.Console.Grep;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.Scene;
using Mercraft.Core.Tiles;
using Mercraft.Infrastructure.Dependencies;

namespace Assets.Scripts.Console.Commands
{
    public class SceneCommand: ICommand
    {
        private readonly IContainer _container;
        public string Description { get { return "scene details"; } }

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
                var tileLoader = _container.Resolve<IPositionListener>() as TileManager;
                var tile = tileLoader.CurrentTile;
                var currentGeoPosition = GeoProjection.ToGeoCoordinate(tileLoader.RelativeNullPoint, tileLoader.CurrentPosition);

                if (arguments["f"] != null)
                    FindModel(tile.Scene, long.Parse((string)arguments["f"]), response);
                else if (arguments["d"] != null)
                {
                    var tileSize = int.Parse((string)arguments["d"]);
                    DumpScene(_container.Resolve<ISceneBuilder>(), currentGeoPosition, tileSize, response);
                }
                else if(arguments["i"] != null)
                    GeneralInfo(tileLoader, currentGeoPosition, response);
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
                if (area.Id != id) continue;
                response.AppendFormat("Found area: {0}\n", area);
                foreach (var point in area.Points)
                {
                    response.AppendLine(point.ToString());
                }
                return;
            }

            foreach (var way in scene.Ways)
            {
                if (way.Id != id) continue;
                response.AppendFormat("Found way: {0}\n", way);
                foreach (var point in way.Points)
                {
                    response.AppendLine(point.ToString());
                }
                return;
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

        private void GeneralInfo(TileManager tileLoader, GeoCoordinate currentGeoPosition, StringBuilder response)
        {
            var tile = tileLoader.CurrentTile;
            var scene = tile.Scene;
            response.AppendFormat("Geo position: {0}\n", currentGeoPosition);
            response.AppendFormat("Map position: {0}\n", tileLoader.CurrentPosition);
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
