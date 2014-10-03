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
            if (mergeDeclarations)
                return _styles.GetMergedRule(model);

            return _styles.GetCollectedRule(model);
        }
    }
}
