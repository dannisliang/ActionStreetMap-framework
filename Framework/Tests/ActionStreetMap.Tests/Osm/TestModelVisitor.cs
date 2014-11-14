using System;
using System.Collections.Generic;
using ActionStreetMap.Core.Scene;
using ActionStreetMap.Core.Scene.Models;

namespace ActionStreetMap.Tests.Osm
{
    public class TestModelVisitor: IModelVisitor
    {
        public List<Area> Areas = new List<Area>();
        public List<Way> Ways = new List<Way>();
        public List<Node> Nodes = new List<Node>();
        public List<Canvas> Canvases = new List<Canvas>();

        public void VisitTile(Tile tile)
        {
            
        }

        public void VisitArea(Area area)
        {
            Areas.Add(area);
        }

        public void VisitWay(Way way)
        {
            Ways.Add(way);
        }

        public void VisitNode(Node node)
        {
           Nodes.Add(node);
        }

        public void VisitCanvas(Canvas canvas)
        {
            Canvases.Add(canvas);
        }
    }
}
