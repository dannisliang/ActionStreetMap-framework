using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime.Tree;
using Mercraft.Core.MapCss.Domain;

namespace Mercraft.Core.MapCss.Visitors
{
    public class MapCssVisitor : IMapCssVisitor
    {
        private readonly List<IMapCssVisitor> _visitors;

        public MapCssVisitor()
        {
            _visitors = new List<IMapCssVisitor>()
            {
                new SelectorMapCssVisitor(),
                new DeclarationMapCssVisitor()
            };
        }

        public MapCssVisitor(List<IMapCssVisitor> visitors)
        {
            _visitors = visitors;
        }

        public Stylesheet Visit(CommonTree tree)
        {
            var stylesheet = new Stylesheet();
            foreach (CommonTree child in tree.Children)
            {
                if (child.Text == "RULE")
                {
                   var rule = VisitRule(child);
                    stylesheet.Rules.Add(rule);
                }
            }

            return stylesheet;
        }

        public Rule VisitRule(CommonTree ruleTree)
        {
            var rule = new Rule();
            for (int i = 0; i < ruleTree.Children.Count; i++)
            {
                var tree = ruleTree.Children[i] as CommonTree;
                if (tree.Text == "SIMPLE_SELECTOR")
                {
                    rule.Selectors.Add(VisitSelector(tree));
                }
                else
                {
                    // declarations
                    if (tree != null && tree.Text == "{")
                    {
                        int declarationSelectorIdx = 0;
                        while (tree.ChildCount > declarationSelectorIdx && tree.Children[declarationSelectorIdx].Text == "DECLARATION")
                        {
                            var declarationTree = tree.Children[declarationSelectorIdx] as CommonTree;

                            rule.Declarations.Add(VisitDeclaration(declarationTree));
                            declarationSelectorIdx++;
                        }
                    }
                }
            }
            return rule;
        }


        public Selector VisitSelector(CommonTree selectorTree)
        {
            return _visitors.Select(visitor => visitor.VisitSelector(selectorTree))
                            .FirstOrDefault(declaration => declaration != null);
        }

        public Declaration VisitDeclaration(CommonTree declarationTree)
        {
            return _visitors.Select(visitor => visitor.VisitDeclaration(declarationTree))
                            .FirstOrDefault(declaration => declaration != null);
        }
    }
}
