namespace Mercraft.Core.Scene.Models
{
    public class Canvas : Model
    {
        public override bool IsClosed
        {
            get { return false; }
        }

        public override void Accept(IModelVisitor visitor)
        {
            visitor.VisitCanvas(this);
        }
    }
}
