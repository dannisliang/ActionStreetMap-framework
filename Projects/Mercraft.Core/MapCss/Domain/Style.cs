using System.Collections.Generic;
using System.Linq;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Domain
{
    public class Style
    {
        /// <summary>
        ///     True if all selectors should be applicable to given model
        /// </summary>
        public bool MatchAll { get; set; }

        /// <summary>
        ///     List of selectors.
        /// </summary>
        public IList<Selector> Selectors { get; set; }

        /// <summary>
        ///     List of declarations.
        /// </summary>
        public IList<Declaration> Declarations { get; set; }


        public Style()
        {
            Selectors = new List<Selector>();
            Declarations = new List<Declaration>();
        }

        public bool IsApplicable(Model model)
        {
            if (MatchAll)
                return Selectors.All(s => s.IsApplicable(model));

            // NOTE closed selector should be checked in both cases
            var closedSelector = Selectors.FirstOrDefault(s => s.IsClosed);
            var isClosedPassed = true;
            if (closedSelector != null)
            {
                isClosedPassed = closedSelector.IsApplicable(model);
            }

            return isClosedPassed && Selectors.Any(s => s.IsApplicable(model));
        }
    }
}