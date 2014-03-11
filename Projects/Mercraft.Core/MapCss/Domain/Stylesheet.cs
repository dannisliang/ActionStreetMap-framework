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
        public IList<Rule> Rules { get; set; }

        public Stylesheet()
        {
            Rules = new List<Rule>();
        }

        public Rule GetRule(Model model)
        {
            return Rules.FirstOrDefault(r => r.IsApplicable(model));
        }
    }
}
