using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Mercraft.Core.Scene.Models;
using Mercraft.Core.Utilities;
using UnityEngine;

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
                    PushToNum();
                    break;
                case "color":
                    PushToColor();
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

        private void PushToNum()
        {
            Expression<Func<string, float>> toFloat = s => float.Parse(SanitizeHelper.SanitizeFloat(s));
            _expressions.Push(toFloat);
        }

        private void PushToColor()
        {
            Expression<Func<string, Color32>> toColor = s => ColorUtility.FromUnknown(s);
            _expressions.Push(toColor);
        }

        #endregion

    }
}
