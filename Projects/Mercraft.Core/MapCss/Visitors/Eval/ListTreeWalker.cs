using System.Collections.Generic;
using Antlr.Runtime.Tree;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Visitors.Eval
{
    public class ListTreeWalker : ITreeWalker
    {
        private readonly List<string> _values = new List<string>();

        public ListTreeWalker(CommonTree tree)
        {
            foreach (var child in tree.Children)
            {
                _values.Add(child.Text);
            }
        }

        public T Walk<T>(Model model)
        {
            return (T) (object) _values;
        }
    }
}