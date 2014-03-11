using System;
using System.Linq.Expressions;
using Antlr.Runtime.Tree;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Visitors.Eval
{
    /// <summary>
    /// Naive implementation of Eval expression builder
    /// </summary>
    public class EvalWalker
    {
        private OperationStack _opStack;
        private ParameterExpression _param = Expression.Parameter(typeof(Model), "model");
        private CommonTree _tree;

        public EvalWalker(CommonTree tree)
        {
            _tree = tree;
            _opStack = new OperationStack(_param);
        }

        public T Walk<T>(Model model)
        {
            var operation = _tree.Children[0] as CommonTree;

            VisitOperation(operation);

            var expression = _opStack.Pop();

            // TODO cache compiled lambda somehow
            Expression<Func<Model, T>> lambda = Expression.Lambda<Func<Model, T>>(
                    expression, new ParameterExpression[] { _param });

            return lambda.Compile().Invoke(model);
        }

        private void VisitOperation(CommonTree tree)
        {
            switch (tree.ChildCount)
            {
                case 0:
                    VisitLeaf(tree);
                    break;
                case 1:
                     VisitUnary(tree);
                    break;
                case 2:
                    VisitBinary(tree);
                    break;
                default:
                    VisitMulti(tree);
                    break;
            }
        }

        private void VisitUnary(CommonTree tree)
        {
            if (tree.ChildCount > 0)
            {
                VisitOperation(tree.Children[0] as CommonTree);
                _opStack.PushUnary(tree.Text);       
            }
            else
            {
                VisitLeaf(tree);
            }
        }

        private void VisitBinary(CommonTree binaryTree)
        {
            VisitOperation(binaryTree.Children[0] as CommonTree);
            VisitOperation(binaryTree.Children[1] as CommonTree);
            _opStack.PushBinary(binaryTree.Text);
        }


        private void VisitLeaf(CommonTree tree)
        {
            _opStack.PushConstant(tree.Text);
        }

        private void VisitMulti(CommonTree tree)
        {
            throw new NotImplementedException();
        }
    }
}
