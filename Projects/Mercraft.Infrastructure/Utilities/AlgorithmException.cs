using System;

namespace Mercraft.Infrastructure.Utilities
{
    /// <summary>
    ///     Used to highlight bugs in algortihms use in app
    /// </summary>
    public class AlgorithmException: Exception
    {
        public AlgorithmException(string message)
            : base(message) { }
    }
}
