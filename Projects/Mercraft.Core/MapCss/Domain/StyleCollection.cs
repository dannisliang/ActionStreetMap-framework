using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    internal class StyleCollection
    {
        private List<Style> _canvasStyles = new List<Style>(1);
        private List<Style> _areaStyles = new List<Style>(16);
        private List<Style> _wayStyles = new List<Style>(16);
        private List<Style> _nodeStyles = new List<Style>(64);

        private List<Style>  _combinedStyles = new List<Style>(16);

        private int _count = 0;

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public void Add(Style style)
        {
            // NOTE store different styles in different collections to increase
            // lookup performance. However, it limits selectors to be the same type
            // I think it's ok for now
            if (style.Selectors.All(s => s is NodeSelector))
            {
                _nodeStyles.Add(style);
            }
            else if (style.Selectors.All(s => s is AreaSelector))
            {
                _areaStyles.Add(style);
            }
            else if (style.Selectors.All(s => s is WaySelector))
            {
                _wayStyles.Add(style);
            }
            else if (style.Selectors.All(s => s is CanvasSelector))
            {
                _canvasStyles.Add(style);
            }
            else
            {
                _combinedStyles.Add(style);
            }

            _count++;
        }

        public Rule GetMergedRule(Model model)
        {
            var styles = GetModelStyles(model);
            var rule = new Rule(model);
            for (int i = 0; i < styles.Count; i++)
                MergeDeclarations(styles[i], rule, model);

            for (int i = 0; i < _combinedStyles.Count; i++)
                MergeDeclarations(_combinedStyles[i], rule, model);

            return rule;
        }

        public Rule GetCollectedRule(Model model)
        {
            var styles = GetModelStyles(model);
            var rule = new Rule(model);
            for (int i = 0; i < styles.Count; i++)
                CollectDeclarations(styles[i], rule, model);

            for (int i = 0; i < _combinedStyles.Count; i++)
                CollectDeclarations(_combinedStyles[i], rule, model);

            return rule;
        }

        public List<Style> GetModelStyles(Model model)
        {
            if (model is Node)
                return _nodeStyles;

            if (model is Area)
                return _areaStyles;

            if (model is Way)
                return _wayStyles;

            return _canvasStyles;
        }

        #region Declaration processing

        private Rule MergeDeclarations(Style style, Rule rule, Model model)
        {
            if (!style.IsApplicable(model))
                return rule;

            foreach (var ruleDeclarations in style.Declarations)
            {
                var declaration = rule.Declarations.SingleOrDefault(d => d.Qualifier == ruleDeclarations.Qualifier);
                if (declaration != null)
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

        #endregion
    }
}
