
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core;
using Mercraft.Core.Algorithms;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.Scene.Models;
using Mercraft.Explorer.Helpers;
using UnityEngine;

namespace Mercraft.Explorer.Builders
{
    public class SolidModelBuilder : ModelBuilder
    {
        public override void BuildArea(GeoCoordinate center, GameObject gameObject, Rule rule, Area area)
        {
            // TODO remove this assertion after handling this case above
            if (area.Points.Length < 3)
            {
                Debug.LogError("Area contains less than 3 points: " + area);
                return;
            }

            gameObject.name = String.Format("Solid {0}", area);
            BuildModel(center, gameObject, rule, area, area.Points.ToList());
        }

        public override void BuildWay(GeoCoordinate center, GameObject gameObject, Rule rule, Way way)
        {
            // TODO remove this assertion after handling this case above
            if (way.Points.Length < 3)
            {
                Debug.LogError("Way contains less than 3 points: " + way);
                return;
            }

            gameObject.name = String.Format("Solid {0}", way);
            BuildModel(center, gameObject, rule, way, way.Points.ToList());
        }

        private void BuildModel(GeoCoordinate center, GameObject gameObject, Rule rule, Model model, IList<GeoCoordinate> coordinates)
        {
            var height = 10f;//rule.GetHeight(model);
            try
            {
                height = rule.GetHeight(model);
            }
            catch(Exception ex)
            {
                Trace.Error("Unable to get height:" + model, ex);
                throw;
            }


            var floor = rule.GetZIndex(model);
            var top = floor + height;

            var verticies = PolygonHelper.GetVerticies2D(center, coordinates);

            var mesh = new Mesh();
            mesh.vertices = PolygonHelper.GetVerticies3D(verticies, top, floor);
            mesh.uv = PolygonHelper.GetUV(verticies);
            mesh.triangles = PolygonHelper.GetTriangles3D(verticies);

            var meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh.Clear();
            meshFilter.mesh = mesh;
            meshFilter.mesh.RecalculateNormals();
        }
    }
}
