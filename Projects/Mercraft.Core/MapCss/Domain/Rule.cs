using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core.MapCss.Visitors.Eval;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Core.MapCss.Domain
{
    public class Rule
    {
        /// <summary>
        /// List of declarations.
        /// </summary>
        public IList<Declaration> Declarations { get; set; }

        public Rule()
        {
            Declarations = new List<Declaration>();
        }

        public bool IsApplicable
        {
            get
            {
                return Declarations.Count > 0;
            }
        }

        public T EvaluateDefault<T>(Model model, string qualifier, T @default)
        {
            Assert();
            var declaration = Declarations.SingleOrDefault(d => d.Qualifier == qualifier);

            if (declaration == null)
                return @default;

            if (declaration.IsEval)
                return declaration.Evaluator.Walk<T>(model);

            return (T)Convert.ChangeType(declaration.Value, typeof(T));
        }

        public T Evaluate<T>(Model model, string qualifier)
        {
            Assert();
            var declaration = Declarations.SingleOrDefault(d => d.Qualifier == qualifier);

            if (declaration == null)
                throw new ArgumentException(String.Format("Declaration '{0}' not found for '{1}'",
                    qualifier, model), qualifier);

            if (declaration.IsEval)
            {
                return declaration.Evaluator.Walk<T>(model);
            }

            return (T)Convert.ChangeType(declaration.Value, typeof(T));
        }

        private void Assert()
        {
            if (!IsApplicable)
                throw new InvalidOperationException("Rule isn't applicable!");
        }
    }
}
