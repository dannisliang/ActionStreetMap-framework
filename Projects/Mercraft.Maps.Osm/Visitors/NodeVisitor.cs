using Mercraft.Core.Scene;
using Mercraft.Infrastructure.Utilities;
using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Visitors
{
    /// <summary>
    ///     Node visitor.
    /// </summary>
    public class NodeVisitor: ElementVisitor
    {
        /// <inheritdoc />
        public NodeVisitor(IModelVisitor modelVisitor, IObjectPool objectPool)
            : base(modelVisitor, objectPool)
        {
        }

        /// <inheritdoc />
        public override void VisitNode(Node node)
        {
            if (node.Tags != null)
            {
                ModelVisitor.VisitNode(new Core.Scene.Models.Node
                {
                    Id = node.Id,
                    Point = node.Coordinate,
                    Tags = node.Tags
                });
            }
        }
    }
}
