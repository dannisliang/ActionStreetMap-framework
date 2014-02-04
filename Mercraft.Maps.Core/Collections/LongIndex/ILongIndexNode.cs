
namespace Mercraft.Maps.Core.Collections.LongIndex
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
