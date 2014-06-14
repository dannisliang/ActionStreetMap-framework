using Mercraft.Models.Roads.Builders;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadBehavior : MonoBehaviour
    {
        public void Attach(RoadSettings settings)
        {
            SimpleRoadBuilder.Build(gameObject, settings);
        }
    }
}