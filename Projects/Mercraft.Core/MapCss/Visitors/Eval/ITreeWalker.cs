using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercraft.Core.Scene.Models;

namespace Mercraft.Core.MapCss.Visitors.Eval
{
    public interface ITreeWalker
    {
        T Walk<T>(Model model);
    }
}
