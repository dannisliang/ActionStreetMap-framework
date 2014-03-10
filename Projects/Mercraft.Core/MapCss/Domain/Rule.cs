using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    public class Rule
    {
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
            return Selectors.All(s => s.IsApplicable(model));
        }

        public T Evaluate<T>(Model model, string qualifier)
        {
            var declaration = Declarations.Single(d => d.Qualifier == qualifier);

            // TODO implement eval
            if (declaration.IsEval)
            {
                throw new NotImplementedException("Eval isn't implemented");
            }

            return (T) Convert.ChangeType(declaration.Value, typeof (T));
        }
    }
}
