using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    public class Rule
    {
        private readonly Model _model;

        /// <summary>
        /// List of declarations.
        /// </summary>
        public IList<Declaration> Declarations { get; set; }

        public Rule(Model model)
        {
            _model = model;
            Declarations = new List<Declaration>();
        }

        public bool IsApplicable
        {
            get
            {
                return Declarations.Count > 0;
            }
        }

        public T EvaluateDefault<T>(string qualifier, T @default)
        {
            Assert();
            var declaration = Declarations.SingleOrDefault(d => d.Qualifier == qualifier);

            if (declaration == null)
                return @default;

            if (declaration.IsEval)
                return declaration.Evaluator.Walk<T>(_model);

            return (T)Convert.ChangeType(declaration.Value, typeof(T));
        }

        public T Evaluate<T>(string qualifier)
        {
            return Evaluate(qualifier, v => (T)Convert.ChangeType(v, typeof(T)));
        }

        public List<T> EvaluateList<T>(string qualifier)
        {
            return EvaluateList(qualifier, v => (T)Convert.ChangeType(v, typeof(T)));
        }

        public List<T> EvaluateList<T>(string qualifier, Func<string, T> converter)
        {
            var declarations = Declarations.Where(d => d.Qualifier == qualifier);
            var values = new List<T>();
            foreach (var declaration in declarations)
            {
                values.Add(declaration.IsEval ? 
                    declaration.Evaluator.Walk<T>(_model) :
                    converter(declaration.Value));
            }

            return values;
        }

        public T Evaluate<T>(string qualifier, Func<string, T> converter)
        {
            Assert();
            var declaration = Declarations.SingleOrDefault(d => d.Qualifier == qualifier);

            if (declaration == null)
                throw new ArgumentException(String.Format("Declaration '{0}' not found for '{1}'",
                    qualifier, _model), qualifier);

            if (declaration.IsEval)
            {
                return declaration.Evaluator.Walk<T>(_model);
            }

            return converter(declaration.Value);
        }

        private void Assert()
        {
            if (!IsApplicable)
                throw new InvalidOperationException("Rule isn't applicable!");
        }
    }
}
