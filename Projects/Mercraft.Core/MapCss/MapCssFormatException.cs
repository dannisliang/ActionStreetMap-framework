using System;

namespace Mercraft.Core.MapCss
{
    public class MapCssFormatException: Exception
    {
        public object Tree { get; private set; }
        public MapCssFormatException(object tree, string message)
            : base(message)
        {
            Tree = tree;
        }
    }
}
