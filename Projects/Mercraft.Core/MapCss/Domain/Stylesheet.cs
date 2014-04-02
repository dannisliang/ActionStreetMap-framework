using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    public class Stylesheet
    {
        // TODO make it private and introduce AddRule method
        /// <summary>
        /// Holds a list of all MapCSS rules.
        /// </summary>
        public IList<Style> Styles { get; set; }

        public Stylesheet()
        {
            Styles = new List<Style>();
        }

        public Rule GetRule(Model model)
        {
            return Styles.Aggregate(new Rule(), (r, s) => MergeDeclarations(s, r, model));
        }

        private Rule MergeDeclarations(Style style, Rule rule, Model model)
        {
            if (!style.IsApplicable(model)) 
                return rule;

            foreach (var ruleDeclarations in style.Declarations)
            {
                var declaration = rule.Declarations.SingleOrDefault(d => d.Qualifier == ruleDeclarations.Qualifier);
                if (declaration!= null)
                {
                    declaration.Value = ruleDeclarations.Value;
                    declaration.Evaluator = ruleDeclarations.Evaluator;
                    declaration.IsEval = ruleDeclarations.IsEval;
                }
                else
                {
                    // Should copy Declaration
                    rule.Declarations.Add(new Declaration()
                    {
                        Qualifier = ruleDeclarations.Qualifier,
                        Value = ruleDeclarations.Value,
                        Evaluator = ruleDeclarations.Evaluator,
                        IsEval = ruleDeclarations.IsEval
                    });
                }
            }
            return rule;
        }
    }
}
