using System.Collections.Generic;

namespace Mercraft.Core.MapCss.Domain
{
    public class Stylesheet
    {
        /// <summary>
        /// Holds a list of all MapCSS rules.
        /// </summary>
        public IList<Rule> Rules { get; set; }

        public Stylesheet()
        {
            Rules = new List<Rule>();
        }

    }
}
