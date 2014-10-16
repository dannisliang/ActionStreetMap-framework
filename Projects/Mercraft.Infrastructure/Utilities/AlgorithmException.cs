using System;

namespace Mercraft.Infrastructure.Utilities
{
    /// <summary>
    ///     Used to highlight bugs in algortihms use in app
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
    [Serializable]
    public class AlgorithmException: Exception
    {
        public AlgorithmException(string message)
            : base(message) { }
    }
}
