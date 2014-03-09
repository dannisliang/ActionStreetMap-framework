using Antlr.Runtime.Tree;
using Mercraft.Core.MapCss.Domain;

namespace Mercraft.Core.MapCss.Visitors
{
    public class MapCssVisitorBase: IMapCssVisitor
    {
        public virtual Stylesheet Visit(CommonTree tree)
        {
            return null;
        }

        public virtual Rule VisitRule(CommonTree ruleTree)
        {
            return null;
        }

        public virtual Selector VisitSelector(CommonTree selectorTree)
        {
            return null;
        }

        public virtual Declaration VisitDeclaration(CommonTree declarationTree)
        {
            return null;
        }
    }
}