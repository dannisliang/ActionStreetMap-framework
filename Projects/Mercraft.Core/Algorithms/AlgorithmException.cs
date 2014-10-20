using System;

namespace Mercraft.Core.Algorithms
{
    /// <summary>
    ///     Used to highlight bugs in algortihms use in app.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
    [Serializable]
    public class AlgorithmException: Exception
    {
        /// <summary>
        ///     Creates AlgorithmException.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public AlgorithmException(string message)
            : base(message) { }
    }
}
