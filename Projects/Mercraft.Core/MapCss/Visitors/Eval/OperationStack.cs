using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Visitors.Eval
{
    internal class OperationStack
    {
        private readonly ParameterExpression _param;
        private readonly Stack<Expression> _expressions = new Stack<Expression>();

        public OperationStack(ParameterExpression param)
        {
            _param = param;
        }

        public void Push(Expression expression)
        {
            _expressions.Push(expression);
        }

        public void PushUnary(string opName)
        {
            switch (opName)
            {
                case "tag":
                    PushTagSelector();
                    break;
                case "num":
                    PushToInt();
                    break;
                default:
                    throw new NotSupportedException(String.Format("Unary operation {0} is not supported", opName));
            }

            bool wrap = Count() > 1;
            if (wrap)
            {
                var outer = Pop();
                var inner = Pop();
                Push(Expression.Invoke(outer, inner));
            }
        }

        public void PushBinary(string opName)
        {
            var left = Pop();
            var right = Pop();
            switch (opName)
            {
                case "*":
                    PushMult(left, right);
                    break;
                default:
                    throw new NotSupportedException(String.Format("Binary operation {0} is not supported", opName));
            }
        }  

        public void PushConstant(string value)
        {
            _expressions.Push(Expression.Constant(value));
        }

        public Expression Pop()
        {
            return _expressions.Pop();
        }

        public int Count()
        {
            return _expressions.Count;
        }

        #region Supported Operations

        private void PushMult(Expression left, Expression right)
        {
            var binaryExpr = Expression.Multiply(left, right);
            Push(binaryExpr);
        }

        private void PushTagSelector()
        {
            var tagKey = (_expressions.Pop() as ConstantExpression).Value.ToString();
            Expression<Func<Model, string>> tagSelector = m => m.Tags.First(t => t.Key == tagKey).Value;
            _expressions.Push(Expression.Invoke(tagSelector, _param));
        }

        private void PushToInt()
        {
            Expression<Func<string, int>> toInt = s => int.Parse(s);
            _expressions.Push(toInt);
        }

        #endregion

    }
}
