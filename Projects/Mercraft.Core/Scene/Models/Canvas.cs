namespace Mercraft.Core.Scene.Models
{
    public class Canvas : Model
    {
        public override bool IsClosed
        {
            get { throw new System.NotImplementedException(); }
        }

        public override void Accept(IModelVisitor visitor)
        {
            visitor.VisitCanvas(this);
        }
    }
}
