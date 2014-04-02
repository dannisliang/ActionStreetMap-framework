using System.Collections.Generic;
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
        private readonly byte _r;
        private readonly byte _g;
        private readonly byte _b;
       
        public ColorTreeWalker(CommonTree tree)
        {
            _r = byte.Parse(tree.Children[0].Text);
            _g = byte.Parse(tree.Children[1].Text);
            _b = byte.Parse(tree.Children[2].Text);
        }

        public T Walk<T>(Model model)
        {         
            // TODO this looks ugly
            return (T) (object) new Color32(_r, _g, _b, 1);
        }
    }
}
