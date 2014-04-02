using System.Linq.Expressions;
using Antlr.Runtime.Tree;
using Mercraft.Core.MapCss.Domain;

namespace Mercraft.Core.MapCss.Visitors
{
    public interface IMapCssVisitor
    {
        Stylesheet Visit(CommonTree tree);

        Style VisitStyle(CommonTree ruleTree);

        Selector VisitSelector(CommonTree selectorTree, string selectorType);

        Declaration VisitDeclaration(CommonTree declarationTree);
    }
}
