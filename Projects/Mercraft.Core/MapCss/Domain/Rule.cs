using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    public class Rule
    {
        private Model _model;
        internal Model Model
        {
            set
            {
                _model = value;
            }
        }

        /// <summary>
        /// List of declarations.
        /// </summary>
        public Dictionary<string, Declaration> Declarations { get; set; }

        public Rule(Model model)
        {
            _model = model;
            Declarations = new Dictionary<string, Declaration>(4);
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

            if (!Declarations.ContainsKey(qualifier))
                return @default;

            var declaration = Declarations[qualifier];

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
            var listDeclaration = (ListDeclaration) Declarations[qualifier];
            var values = new List<T>();
            foreach (var declaration in listDeclaration.Items)
            {
                values.Add(declaration.IsEval ? 
                    declaration.Evaluator.Walk<T>(_model) :
                    converter(declaration.Value));
            }

            return values;
            return null;
        }

        public T Evaluate<T>(string qualifier, Func<string, T> converter)
        {
            Assert();

            if (!Declarations.ContainsKey(qualifier))
                throw new ArgumentException(String.Format(ErrorStrings.StyleDeclarationNotFound,
                    qualifier, _model), qualifier);

            var declaration = Declarations[qualifier];

            if (declaration.IsEval)
            {
                return declaration.Evaluator.Walk<T>(_model);
            }

            return converter(declaration.Value);
        }

        private void Assert()
        {
            if (!IsApplicable)
                throw new InvalidOperationException(ErrorStrings.RuleNotApplicable);
        }
    }
}
