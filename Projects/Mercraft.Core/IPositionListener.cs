using UnityEngine;

namespace Mercraft.Core
{
    public interface IPositionListener
    {
        void OnPositionChanged(Vector2 position);
    }
}
