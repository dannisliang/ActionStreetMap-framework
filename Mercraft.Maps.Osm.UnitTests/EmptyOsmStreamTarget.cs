using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mercraft.Maps.Osm.Entities;
using Mercraft.Maps.Osm.Streams;

namespace Mercraft.Maps.Osm.UnitTests
{
    public class EmptyOsmStreamTarget: OsmStreamTarget
    {
        public EmptyOsmStreamTarget(OsmStreamSource source) : base(source)
        {
        }

        public override void Initialize()
        {
            
        }

        public override void AddNode(Node simpleNode)
        {
            //Console.WriteLine(simpleNode);
        }

        public override void AddWay(Way simpleWay)
        {
            //Console.WriteLine(simpleWay);
        }

        public override void AddRelation(Relation simpleRelation)
        {
            //Console.WriteLine(simpleRelation);
        }
    }
}
