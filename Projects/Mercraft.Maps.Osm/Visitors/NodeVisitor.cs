using Mercraft.Core.Scene;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Visitors
{
    public class NodeVisitor: ElementVisitor
    {
        public NodeVisitor(IModelVisitor modelVisitor)
            : base(modelVisitor)
        {
        }

        public override void VisitNode(Node node)
        {
            if (node.Tags != null)
            {
                ModelVisitor.VisitNode(new Core.Scene.Models.Node()
                {
                    Id = node.Id,
                    Point = node.Coordinate,
                    Tags = node.Tags
                });
            }
        }
    }
}
