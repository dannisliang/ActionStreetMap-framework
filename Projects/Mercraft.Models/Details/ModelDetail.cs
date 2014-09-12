using UnityEngine;

namespace Mercraft.Models.Details
{
    /// <summary>
    ///     Represents detail model
    /// </summary>
    public class ModelDetail
    {
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }

        public string Path { get; set; }
    }
}
