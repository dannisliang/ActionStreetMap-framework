using Mercraft.Core.Scene;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Visitors
{
    public class NodeVisitor: ElementVisitor
    {
        public NodeVisitor(IScene scene) : base(scene)
        {
        }

        public override void VisitNode(Node node)
        {
            if (node.Tags != null)
            {
                Scene.AddNode(new Core.Scene.Models.Node()
                {
                    Id = node.Id,
                    Point = node.Coordinate,
                    Tags = node.Tags
                });
            }
        }
    }
}
