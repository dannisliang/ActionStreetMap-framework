using Antlr.Runtime.Tree;
using Mercraft.Core.MapCss.Domain;

namespace Mercraft.Core.MapCss.Visitors
{
    public class DeclarationMapCssVisitor: MapCssVisitorBase
    {
        public override Declaration VisitDeclaration(CommonTree declarationTree)
        {
            var declaration = new Declaration();
            if (declarationTree == null)
            {
                throw new MapCssFormatException(declarationTree, "Declaration tree not valid!");
            }

            declaration.Qualifier = declarationTree.Children[0].Text;
            declaration.Value = declarationTree.Children[1].Text;

            if (declaration.Value == "EVAL_CALL")
            {
                declaration.IsEval = true;
                declaration.Value = (declarationTree.Children[1] as CommonTree).Children[0].Text;
            }

            return declaration;
        }
    }
}
