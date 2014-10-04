using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
                    var style = VisitStyle(child);
                    stylesheet.AddStyle(style);
                }
            }

            return stylesheet;
        }

        public Style VisitStyle(CommonTree ruleTree)
        {
            var style = new Style();
            for (int i = 0; i < ruleTree.Children.Count; i++)
            {
                var tree = ruleTree.Children[i] as CommonTree;
                if (tree.Text == "SIMPLE_SELECTOR")
                {
                    var selectorType = (tree.Children[0] as CommonTree).Text;

                    // NOTE canvas is special case: it doesn't have selectors
                    // but we want to use it later to be consistent
                    if (selectorType != "canvas")
                    {
                        int selectorIdx = 1;
                        var selectors = new List<Selector>();
                        while (tree.ChildCount > selectorIdx)
                        {
                            var selectorTree = tree.Children[selectorIdx] as CommonTree;
                            selectors.Add(VisitSelector(selectorTree, selectorType));
                            selectorIdx++;
                        }
                        style.Selectors.Add(selectors.Count > 1
                            ? new AndSelector(selectors)
                            : selectors.Single());
                    }
                    else
                    {
                         style.Selectors.Add(new CanvasSelector());
                    }
                }
                else
                {
                    style.MatchAll = i == 1;
                    
                    // declarations
                    if (tree != null && tree.Text == "{")
                    {
                        int declarationSelectorIdx = 0;
                        while (tree.ChildCount > declarationSelectorIdx && tree.Children[declarationSelectorIdx].Text == "DECLARATION")
                        {
                            var declarationTree = tree.Children[declarationSelectorIdx] as CommonTree;
                            var declaration = VisitDeclaration(declarationTree);
                            if (declaration.Value == "VALUE_LIST")
                            {
                                ListDeclaration listDeclaration = null;
                                if (style.Declarations.ContainsKey(declaration.Qualifier))
                                    // NOTE we don't expect duplications of list declaration without VALUE_LIST
                                    listDeclaration = (ListDeclaration) style.Declarations[declaration.Qualifier];
                                else
                                {
                                    listDeclaration = new ListDeclaration();
                                    style.Declarations.Add(declaration.Qualifier, listDeclaration);
                                }
                                listDeclaration.Items.Add(declaration);
                            }
                            else
                            {
                                style.Declarations.Add(declaration.Qualifier, declaration);
                            }
                            declarationSelectorIdx++;
                        }
                    }
                }
            }
            return style;
        }


        public Selector VisitSelector(CommonTree selectorTree, string selectorType)
        {
            return _visitors.Select(visitor => visitor.VisitSelector(selectorTree, selectorType))
                            .FirstOrDefault(declaration => declaration != null);
        }

        public Declaration VisitDeclaration(CommonTree declarationTree)
        {
            return _visitors.Select(visitor => visitor.VisitDeclaration(declarationTree))
                            .FirstOrDefault(declaration => declaration != null);
        }
    }
}
