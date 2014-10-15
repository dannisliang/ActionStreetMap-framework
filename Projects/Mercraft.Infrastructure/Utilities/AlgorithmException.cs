using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Mercraft.Infrastructure.Utilities
{
    /// <summary>
    ///     Used to highlight bugs in algortihms use in app
    /// </summary>
    [Serializable]
    public class AlgorithmException: Exception
    {
        public AlgorithmException(string message)
            : base(message) { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
