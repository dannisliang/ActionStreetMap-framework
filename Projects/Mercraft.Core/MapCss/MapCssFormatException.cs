using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Mercraft.Core.MapCss
{
    [Serializable]
    public class MapCssFormatException: Exception
    {
        public object Tree { get; private set; }

        public MapCssFormatException(object tree, string message)
            : base(message)
        {
            Tree = tree;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // TODO serialize tree if necessary
            base.GetObjectData(info, context);
        }
    }
}
