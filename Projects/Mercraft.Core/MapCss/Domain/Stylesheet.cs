using System.Linq;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    public class Stylesheet
    {
        private StyleCollection _styles;

        public Stylesheet()
        {
            _styles = new StyleCollection();
        }

        public void AddStyle(Style style)
        {
            _styles.Add(style);
        }

        public int Count
        {
            get
            {
                return _styles.Count;
            }
        }

        public Rule GetRule(Model model, bool mergeDeclarations = true)
        {
            if(mergeDeclarations)
                return _styles.GetRule(model, (r, s) => MergeDeclarations(s, r, model));

            return _styles.GetRule(model, (r, s) => CollectDeclarations(s, r, model));
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

        private Rule CollectDeclarations(Style style, Rule rule, Model model)
        {
            if (!style.IsApplicable(model))
                return rule;

            foreach (var ruleDeclarations in style.Declarations)
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
            return rule;
        }
    }
}
