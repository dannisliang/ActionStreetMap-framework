
namespace Mercraft.Maps.Osm.Extensions.LongIndex
{
    internal interface ILongIndexNode
    {
        bool Contains(long number);

        void Add(long number);

        void Remove(long number);

        short Base
        {
            get;
        }
    }
}
