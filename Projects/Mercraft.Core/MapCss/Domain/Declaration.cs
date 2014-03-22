using System.Linq.Expressions;
using Mercraft.Core.MapCss.Visitors.Eval;

namespace Mercraft.Core.MapCss.Domain
{
    public class Declaration
    {
        public string Qualifier { get; set; }
        public string Value { get; set; }

        public bool IsEval { get; set; }
        public ITreeWalker Evaluator { get; set; }
    }
}
