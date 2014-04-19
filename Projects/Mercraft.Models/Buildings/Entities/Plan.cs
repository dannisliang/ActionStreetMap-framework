using System.Collections.Generic;
using Mercraft.Models.Buildings.Utils;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    public class Plan
    {
        public List<Vector2> Points = new List<Vector2>();
        public List<Volume> Volumes = new List<Volume>();
      
        /// <summary>
        /// Gets the ordered points from a volume.
        /// </summary>
        public List<Vector2> GetOrderedPoints(int volumeIndex)
        {
            var orderedPoints = new List<Vector2>();
            int volumeCount = Volumes[volumeIndex].Count;
            for (int i = 0; i < volumeCount; i++)
                orderedPoints.Add(Points[Volumes[volumeIndex].Points[i]]);

            return orderedPoints;
        }

        public int GetFacadeFloorHeight(int volumeIndex, int pointA, int pointB)
        {
            const int returnFloorHeight = 0;
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
                    return Volumes[otherVolume].NumberOfFloors;

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
            int pointIndexBase = Points.Count;
            Points.AddRange(newPoints);

            int newVolumeIndex = AddVolume();
            for (int p = 0; p < numberOfnewPoints; p++)
                Volumes[newVolumeIndex].Add(p + pointIndexBase);

            return Volumes[newVolumeIndex];
        }

        public int AddVolume()
        {
            Volume volume = new Volume();
            volume.NumberOfFloors = Mathf.FloorToInt(volume.Height / BuildingMeasurements.FloorHeightMin);
            Volumes.Add(volume);
            volume.Style = new VolumeStyle();
            return Volumes.Count - 1;
        }

        /// <summary>
        /// returns the connected volume id for a given two area point index values (not the volume index values)
        /// </summary>
        private List<int> GetVolumeIDs(int a, int b)
        {
            List<int> aVolumes = new List<int>();
            int s;
            List<int> returnVolumeIDs = new List<int>();
            for (s = 0; s < Volumes.Count; s++)
            {
                if (Volumes[s].Points.Contains(a))
                    aVolumes.Add(s);
            }
            for (s = 0; s < Volumes.Count; s++)
            {
                if (Volumes[s].Points.Contains(b) && aVolumes.Contains(s))
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
