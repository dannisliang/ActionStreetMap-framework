using Mercraft.Maps.Osm.Entities;

namespace Mercraft.Maps.Osm.Filters
{
    public interface IFilter
    {
        bool Evaluate(Element obj);
    }
}
