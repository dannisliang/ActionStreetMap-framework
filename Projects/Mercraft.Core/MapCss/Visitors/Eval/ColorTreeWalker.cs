using Antlr.Runtime.Tree;
using Mercraft.Core.Scene.Models;
using UnityEngine;

namespace Mercraft.Core.MapCss.Visitors.Eval
{
    /// <summary>
    /// Builds color from RGB representation
    /// </summary>
    public class ColorTreeWalker: ITreeWalker
    {
        private readonly int _r;
        private readonly int _g;
        private readonly int _b;
        
        public ColorTreeWalker(CommonTree tree)
        {
            _r = int.Parse(tree.Children[0].Text);
            _g = int.Parse(tree.Children[1].Text);
            _b = int.Parse(tree.Children[2].Text);
        }

        public T Walk<T>(Model model)
        {
            // TODO this looks ugly
            return (T) (object) new Color(_r, _g, _b);
        }
    }
}
