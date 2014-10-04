using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    public class Stylesheet
    {
        private readonly StyleCollection _styles = new StyleCollection();

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

        public Rule GetModelRule(Model model)
        {
            return _styles.GetMergedRule(model);
        }

        public Rule GetCanvasRule(Canvas canvas)
        {
            return _styles.GetCollectedRule(canvas);
        }

        public void StoreRule(Rule rule)
        {
            _styles.StoreRule(rule);
        }
    }
}
