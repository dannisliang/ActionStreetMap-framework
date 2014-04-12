using System.Collections.Generic;
using Mercraft.Models.Buildings.Enums;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    [System.Serializable]
    public class Plan : ScriptableObject
    {

        public List<Vector2> points = new List<Vector2>();
        public List<Volume> volumes = new List<Volume>();
      

        public int numberOfPoints
        {
            get { return points.Count; }
        }

        public int numberOfVolumes
        {
            get { return volumes.Count; }
        }
    
        /// <summary>
        /// Gets the ordered points from a volume.
        /// </summary>
        public List<Vector2> GetOrderedPoints(int volumeIndex)
        {
            List<Vector2> orderedPoints = new List<Vector2>();
            int volumeCount = volumes[volumeIndex].Count;
            for (int i = 0; i < volumeCount; i++)
                orderedPoints.Add(points[volumes[volumeIndex].points[i]]);

            return orderedPoints;
        }

        public int GetFacadeFloorHeight(int volumeIndex, int pointA, int pointB)
        {
            int returnFloorHeight = 0;
            List<int> volumeIDs = GetVolumeIDs(pointA, pointB);

            if (!volumeIDs.Contains(volumeIndex))
                Debug.LogError("Error, this wall isn't within this volume");

            switch (volumeIDs.Count)
            {
                case 0:
                    Debug.LogError("Error, this wall isn't within this volume");
                    break;

                case 1:
                    return returnFloorHeight;//no adjacent volume - floor height = 0

                case 2:
                    int otherVolume = (volumeIDs.IndexOf(volumeIndex) == 0) ? volumeIDs[1] : volumeIDs[0];
                    return volumes[otherVolume].numberOfFloors;

                default:
                    Debug.LogError("Error, a wall can't have more than one volume");
                    break;
            }

            return returnFloorHeight;
        }

        public Volume AddVolume(Vector2[] newPoints)
        {
            if (!BuildrPolyClockwise.Check(newPoints))
                System.Array.Reverse(newPoints);

            int numberOfnewPoints = newPoints.Length;
            int pointIndexBase = points.Count;
            points.AddRange(newPoints);

            int newVolumeIndex = AddVolume();
            for (int p = 0; p < numberOfnewPoints; p++)
                volumes[newVolumeIndex].Add(p + pointIndexBase);

            return volumes[newVolumeIndex];
        }

        public int AddVolume()
        {
            Volume newVolume = ScriptableObject.CreateInstance<Volume>();
            newVolume.numberOfFloors = Mathf.FloorToInt(newVolume.height / BuildingMeasurements.FLOOR_HEIGHT_MIN);
            volumes.Add(newVolume);
            newVolume.styles = ScriptableObject.CreateInstance<VolumeStyles>();
            return numberOfVolumes - 1;
        }

        public void AddVolume(int a, int b, Vector2[] newPoints)
        {
            int volumeID = AddVolume();

            int pointBase = points.Count;
            int numberOfNewPoints = newPoints.Length;
            points.AddRange(newPoints);

            volumes[volumeID].Add(a);
            for (int np = 0; np < numberOfNewPoints; np++)
            {
                volumes[volumeID].Add(pointBase);
                pointBase++;
            }
            volumes[volumeID].Add(b);
        }

        /// <summary>
        /// returns the connected volume id for a given two area point index values (not the volume index values)
        /// </summary>
        private List<int> GetVolumeIDs(int a, int b)
        {
            List<int> aVolumes = new List<int>();
            int s;
            List<int> returnVolumeIDs = new List<int>();
            for (s = 0; s < numberOfVolumes; s++)
            {
                if (volumes[s].points.Contains(a))
                    aVolumes.Add(s);
            }
            for (s = 0; s < numberOfVolumes; s++)
            {
                if (volumes[s].points.Contains(b) && aVolumes.Contains(s))
                    returnVolumeIDs.Add(s);
            }
            return returnVolumeIDs;
        }


        public int[] GetTrianglesBySectorBase(int sectorIndex)
        {
            Vector2[] orderedPoints = GetOrderedPoints(sectorIndex).ToArray();
            return EarClipper.Triangulate(orderedPoints);
        }
    }
}
