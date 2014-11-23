using System;

namespace ActionStreetMap.Core.MapCss
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
    [Serializable]
    internal class MapCssFormatException: Exception
    {
        public object Tree { get; private set; }

        public MapCssFormatException(string message)
            : base(message)
        {
            
        }

        public MapCssFormatException(object tree, string message)
            : base(message)
        {
            Tree = tree;
        }
    }
}
