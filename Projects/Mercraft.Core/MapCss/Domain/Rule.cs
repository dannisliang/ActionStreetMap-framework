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
            //UnityEngine.Debug.Log("Rule.IsApplicable");
            return Selectors.All(s => s.IsApplicable(model));
        }

        public T Evaluate<T>(Model model, string qualifier)
        {
            var declaration = Declarations.Single(d => d.Qualifier == qualifier);

            if (declaration.IsEval)
                return declaration.Evaluator.Walk<T>(model);         

            return (T) Convert.ChangeType(declaration.Value, typeof (T));
        }
    }
}
