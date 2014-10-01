using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    internal class StyleCollection
    {
        private IList<Style> _canvasStyles = new List<Style>(1);
        private IList<Style> _areaStyles = new List<Style>(16);
        private IList<Style> _wayStyles = new List<Style>(16);
        private IList<Style> _nodeStyles = new List<Style>(64);

        private IList<Style>  _andStyles = new List<Style>(16);

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
                _andStyles.Add(style);
            }

            _count++;
        }

        public Rule GetRule(Model model, Func<Rule,Style,Rule> action)
        {
            return GetModelStyles(model).Aggregate(
                _andStyles.Aggregate(new Rule(model), action), 
                action);
        }

        private IEnumerable<Style> GetModelStyles(Model model)
        {
            if (model is Node)
                return _areaStyles;

            if (model is Area)
                return _areaStyles;

            if (model is Way)
                return _wayStyles;

            return _canvasStyles;
        }
    }
}
