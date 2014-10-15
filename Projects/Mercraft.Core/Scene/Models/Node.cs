
namespace Mercraft.Core.Scene.Models
{
    public class Node: Model
    {
        public GeoCoordinate Point { get; set; }

        public override bool IsClosed
        {
            get { return false; }
        }

        public override void Accept(IModelVisitor visitor)
        {
            visitor.VisitNode(this);
        }
    }
}
