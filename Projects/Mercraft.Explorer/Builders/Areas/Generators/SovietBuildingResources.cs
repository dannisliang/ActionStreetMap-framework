using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercraft.Explorer.Builders.Areas.Generators
{
    public class SovietBuildingResources
    {
        public readonly List<Mesh> Balcony25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Balcony25"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Balcony25Glazed")
        };

        public readonly List<Mesh> Balcony30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Balcony30"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Balcony30Glazed")
        };

        public readonly List<Mesh> Wall25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Wall25")
        };
        public readonly List<Mesh> Wall30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Wall30")
        };
        public readonly List<Mesh> Window25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Window25")
        };
        public readonly List<Mesh> Window30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Window30")
        };

        public readonly List<Mesh> Socle25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Socle25")
        };
        public readonly List<Mesh> Socle30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Socle30")
        };

        public readonly List<Mesh> Entrance25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Entrance25"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Entrance25Roofed"),
        };
        public readonly List<Mesh> Entrance30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Entrance30"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Entrance30Roofed"),
        };
        public readonly List<Mesh> EntranceWall25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWall25"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Window25")
        };
        public readonly List<Mesh> EntranceWall30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWall30"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Window30")
        };
        public readonly List<Mesh> EntranceWallLast25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWallLast25")
        };
        public readonly List<Mesh> EntranceWallLast30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWallLast30")
        };
        public readonly List<Mesh> Attic25 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWallLast25"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Attic25")
        };
        public readonly List<Mesh> Attic30 = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/EntranceWallLast30"),
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Attic30")
        };
        public readonly List<Mesh> RoofFlat = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/Roof")
        };
        public readonly List<Mesh> RoofGabled = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/RoofGabled")
        };
        public readonly List<Mesh> RoofHipped = new List<Mesh>()
        {
            Resources.Load<Mesh>(@"Models/Buildings/Soviet/RoofHipped")
        };
    }
}
