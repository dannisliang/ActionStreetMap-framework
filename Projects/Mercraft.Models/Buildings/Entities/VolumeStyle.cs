using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Models.Buildings.Entities
{
    public class VolumeStyleUnit
    {
        public int StyleId = 0;
        public int FacadeId = 0;
        public int Floors = 0;
        public int EntryId = 0;

        public VolumeStyleUnit(int style, int facade, int numberOfFloors, int entry)
        {
            StyleId = style;
            FacadeId = facade;
            Floors = numberOfFloors;
            EntryId = entry;
        }
    }

    public class VolumeStyle
    {
        private readonly List<int> _styleId = new List<int>();
        private readonly List<int> _facadeId = new List<int>();
        private readonly List<int> _floors = new List<int>();

        public void Clear()
        {
            _styleId.Clear();
            _facadeId.Clear();
            _floors.Clear();
        }

        public void AddStyle(int style, int facade, int numberOfFloors)
        {
            _styleId.Add(style);
            _facadeId.Add(facade);
            _floors.Add(numberOfFloors);
        }   
   
        public VolumeStyleUnit[] GetContentsByFacade(int facadeIndex)
        {
            int numberOfEntries = _styleId.Count;
            var output = new List<VolumeStyleUnit>();
            for (int i = 0; i < numberOfEntries; i++)
            {
                if (_facadeId[i] == facadeIndex)
                    output.Add(new VolumeStyleUnit(_styleId[i], _facadeId[i], _floors[i], i));
            }
            return output.ToArray();
        }

    }
}
