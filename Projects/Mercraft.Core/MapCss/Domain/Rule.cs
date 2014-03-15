using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    public class Rule
    {
        /// <summary>
        /// True if all selectors should be applicable to given model
        /// </summary>
        public bool MatchAll { get; set; }

        /// <summary>
        /// List of selectors.
        /// </summary>
        public IList<Selector> Selectors { get; set; }

        /// <summary>
        /// List of declarations.
        /// </summary>
        public IList<Declaration> Declarations { get; set; }


        public Rule()
        {
            Selectors = new List<Selector>();
            Declarations = new List<Declaration>();
        }

        public bool IsApplicable(Model model)
        {
            return MatchAll?  
                Selectors.All(s => s.IsApplicable(model)):
                Selectors.Any(s => s.IsApplicable(model));
        }

        public T EvaluateDefault<T>(Model model, string qualifier, T @default)
        {
            var declaration = Declarations.SingleOrDefault(d => d.Qualifier == qualifier);

            if (declaration == null)
                return @default;

            if (declaration.IsEval)
                return declaration.Evaluator.Walk<T>(model);

            return (T)Convert.ChangeType(declaration.Value, typeof(T));
        }

        public T Evaluate<T>(Model model, string qualifier)
        {          
            var declaration = Declarations.SingleOrDefault(d => d.Qualifier == qualifier);

            if (declaration == null)
                throw new ArgumentException("Declaration not found!", qualifier);

            if (declaration.IsEval)
                return declaration.Evaluator.Walk<T>(model);

            return (T)Convert.ChangeType(declaration.Value, typeof(T));
        }
    }
}
