using Antlr.Runtime.Tree;
using Mercraft.Core.MapCss.Domain;
using Mercraft.Core.MapCss.Visitors.Eval;

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
                declaration.Evaluator = new EvalTreeWalker(declarationTree.Children[1] as CommonTree);
            }

            if (declaration.Value == "VALUE_RGB")
            {
                declaration.IsEval = true;
                declaration.Evaluator = new ColorTreeWalker(declarationTree.Children[1] as CommonTree);
            }

            return declaration;
        }
    }
}
