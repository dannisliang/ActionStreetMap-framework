using System;
using System.Linq;
using Mercraft.Core.World.Roads;
using Mercraft.Infrastructure.Primitives;
using UnityEngine;

namespace Mercraft.Models.Roads
{
    public class RoadFixBehavior : MonoBehaviour
    {
        // TODO determine the best value or ignore it?
        private const float ClosestPointTreshold = 10;
        private const float Step = 3f;
        private const int MaxCollisionCount = 50;

        private int _collisionCount;

        void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.tag == "osm.road")
            {
                var road = collision.gameObject.GetComponent<RoadBehavior>().Road;

                // limit collision count
                if (++_collisionCount == MaxCollisionCount)
                {
                    Debug.LogError(String.Format("Unable to apply road fix to: {0}, element id: {1}", 
                        name, road.Elements.First()));
                    Destroy(this);
                    return;
                }

                var collisionPoint = collision.contacts[0].point;
                var newPosition = FindOutsidePoint(road, collisionPoint);

                transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);

                // NOTE rotate: we assumed that all models has predifined orientation
                // TODO set specific rotation offset outside
                var direction = collisionPoint - newPosition;
                var targetRotation = Quaternion.FromToRotation(Vector3.forward, Vector3.left) *
                    Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);

                Debug.DrawLine(
                    new Vector3(collisionPoint.x, collisionPoint.y + 0.3f, collisionPoint.z),
                    new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), Color.red, 100, false);
            }
        }

        private Vector3 FindOutsidePoint(Road road, Vector3 collisionPoint)
        {
            // First, find indecies of road element and point which are the closest to collision point
            var indecies = new Tuple<int, int>(0, 0);
            float minDistance = Single.MaxValue;
            for (int j = 0; j < road.Elements.Count; j++)
            {
                var points = road.Elements[j].Points;
                for (int i = 0; i < points.Length; i++)
                {
                    var point = points[i];
                    var distance = point.DistanceTo(collisionPoint.x, collisionPoint.z);
                    // use treshold value to finish this loop earlier
                    if (distance < ClosestPointTreshold)
                        return GetOutsidePoint(road, collisionPoint, j, i);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        indecies.Item1 = j;
                        indecies.Item2 = i;
                    }
                }
            }
            // Then, detect new position outside road using found indecies
            return GetOutsidePoint(road, collisionPoint, indecies.Item1, indecies.Item2);
        }

        private Vector3 GetOutsidePoint(Road road, Vector3 collisionPoint, int roadElementIndex, int pointIndex)
        {
            var roadElement = road.Elements[roadElementIndex];

            var mapPointA = roadElement.Points[pointIndex];
            var mapPointB = pointIndex != roadElement.Points.Length - 1
                ? roadElement.Points[pointIndex + 1]
                : roadElement.Points[pointIndex - 1];
          
            var orthogonal = new Vector3(-(mapPointB.Y - mapPointA.Y), collisionPoint.y, mapPointB.X - mapPointA.X);

            var ba = new Vector3(mapPointB.X - mapPointA.X, collisionPoint.y, mapPointB.Y - mapPointA.Y);
            var cb = new Vector3(collisionPoint.x - mapPointB.X, collisionPoint.y, collisionPoint.z - mapPointB.Y);
            
            // detect sign to found the closest way outside road (left/right side of road)
            var sign = -ba.x * cb.z + ba.z * cb.x;
            if (sign > 0)
            {
                orthogonal.x = -orthogonal.x;
                orthogonal.z = -orthogonal.z;
            }

            orthogonal.Normalize();

            // NOTE this point isn't necessary will be outside of road, we let other collisions happen
            // We cannot use big step, otherwise it will lead object to be too far away from road
            // TODO should step detect on road size to reduce amount of further collisions?
            return collisionPoint + orthogonal * Step;
        }
    }
}
