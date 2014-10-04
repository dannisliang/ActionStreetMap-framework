using System;
using System.Collections.Generic;
using Mercraft.Core.MapCss.Visitors.Eval;

namespace Mercraft.Core.MapCss.Domain
{
    public class Declaration
    {
        public string Qualifier { get; set; }
        public string Value { get; set; }

        public bool IsEval { get; set; }
        public ITreeWalker Evaluator { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}:{1}]:{2}", Qualifier, Value, IsEval);
        }
    }

    public class ListDeclaration : Declaration
    {
        private readonly List<Declaration> _declarations = new List<Declaration>(4);
        public List<Declaration> Items
        {
            get
            {
                return _declarations;
            }
        }
    }
}
