using System;
using Antlr.Runtime.Tree;
using Mercraft.Core.MapCss.Domain.Selectors;

namespace Mercraft.Core.MapCss.Visitors
{
    public class SelectorMapCssVisitor : MapCssVisitorBase
    {
        private const string OperationExistName = "OP_EXIST";

        public override  Selector VisitSelector(CommonTree selectorTree)
        {
            Selector selector = null;
            var selectorAttrTree = selectorTree.Children[1] as CommonTree;
            var selectorType = (selectorTree.Children[0] as CommonTree).Text;
            switch (selectorType)
            {
                case "area":
                    selector = new AreaSelector();
                    break;
                case "node":
                    selector = new NodeSelector();
                    break;
                case "way":
                    selector = new WaySelector();
                    break;
                case "relation":
                    // TODO
                    break;
                default:
                    throw new ArgumentException("Unknow selector type: {0}", selectorType);
            }

            var operation = selectorAttrTree.Children[0].Text;
            if (operation == OperationExistName)
            {
                if (selectorAttrTree.ChildCount != 2)
                {
                    throw new MapCssFormatException(selectorAttrTree, "Wrong 'exist' selector operation");
                }
                selector.Tag = selectorAttrTree.Children[1].Text;
            }
            else
            {
                if (selectorAttrTree.ChildCount != 3)
                {
                    throw new MapCssFormatException(selectorAttrTree,
                        String.Format("Wrong '{0}' selector operation", operation));
                }

                switch (operation)
                {
                    case "=":
                        break;
                    default:
                        throw new MapCssFormatException(selectorAttrTree,
                            String.Format("Not supported selector operation: {0}", operation));
                }

                selector.Tag = selectorAttrTree.Children[1].Text;
                selector.Value = selectorAttrTree.Children[2].Text;
            }
            selector.Operation = operation;

            return selector;
        }
    }
}
