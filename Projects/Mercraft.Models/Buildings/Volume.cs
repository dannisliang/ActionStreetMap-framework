using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mercraft.Models.Buildings
{
    public class Volume : ScriptableObject
    {
        public List<int> points = new List<int>();
        public List<bool> renderFacade = new List<bool>();

        public float height = 10;

        private int _numberOfFloors = 1;
        public VolumeStyles styles;
        public int roofDesignID = 0;

        public void Init()
        {
            styles = ScriptableObject.CreateInstance<VolumeStyles>();
        }

        public int numberOfFloors
        {
            get { return _numberOfFloors; }
            set
            {
                _numberOfFloors = Mathf.Max(1, value);//make sure the minimun value is one - can't have a volume that's 0
                //or cant we? maybe in a patch we add park/garden/plaza stuff
            }
        }

        public void Add(int newInt)
        {
            points.Add(newInt);
            renderFacade.Add(true);
            styles.AddStyle(0, newInt, 1);
        }

        public void Add(int newInt, VolumeStylesUnit[] styleunits)
        {
            points.Add(newInt);
            renderFacade.Add(true);

            foreach (VolumeStylesUnit style in styleunits)
                styles.AddStyle(style.styleID, style.facadeID, style.floors);
        }

        public void AddRange(int[] newInts)
        {
            foreach (int newInt in newInts)
            {
                points.Add(newInt);
                renderFacade.Add(true);
                styles.AddStyle(0, newInt, 1);
            }
        }

        public void Insert(int index, int newInt)
        {
            points.Insert(index, newInt);
            renderFacade.Insert(index, true);
            styles.NudgeFacadeValues(index);
            styles.AddStyle(0, newInt, 1);
        }

        public void RemoveAndUpdate(int pointID)
        {
            Remove(pointID);
            UpdateIndexUponRemoval(pointID);
        }

        public void Remove(int pointID)
        {
            int index = points.IndexOf(pointID);

            points.Remove(pointID);
            renderFacade.RemoveAt(index);
            //styles.UpdatePointIDRemoval(pointID);
            styles.RemoveStyleByFacadeID(pointID);
        }

        public void UpdateIndexUponRemoval(int pointID)
        {
            styles.UpdatePointIDRemoval(pointID);
            int numberOfPoints = points.Count;
            for (int p = 0; p < numberOfPoints; p++)
                if (points[p] > pointID)
                    points[p]--;
        }

        public void RemoveAt(int pointIndex)
        {
            RemoveAndUpdate(points[pointIndex]);
        }

        public int Count
        {
            get { return points.Count; }
        }


        public bool Contains(int i)
        {
            return points.Contains(i);
        }

        public int IndexOf(int i)
        {
            return points.IndexOf(i);
        }

        public int[] ToArray()
        {
            return points.ToArray();
        }

        public int GetWallIndex(int a, int b)
        {
            int size = Count;
            for (int i = 0; i < size; i++)
            {
                int i2 = (i + 1) % size;

                int pointIndexA = points[i];
                int pointIndexB = points[i2];

                if (pointIndexA == a && pointIndexB == b)
                    return i;

                if (pointIndexB == a && pointIndexA == b)
                    return i;
            }

            return -1;
        }
    }
}
