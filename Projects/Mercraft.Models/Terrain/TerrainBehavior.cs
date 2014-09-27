using Mercraft.Infrastructure.Diagnostic;
using UnityEngine;

namespace Mercraft.Models.Terrain
{
    public class TerrainBehaviour: MonoBehaviour
    {
        public ITrace Trace { get; set; }

        private void OnBecameInvisible()
        {
            Trace.Warn("OnBecameInvisible:" + name);
        }
    }
}
