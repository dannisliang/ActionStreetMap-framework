using System.Collections.Generic;

namespace Mercraft.Core.World
{
    public interface IWorldEntry
    {
        long Id { get; }
    }

    public class Collection<T> where T: IWorldEntry
    {
        private Dictionary<long, T> _entries = new Dictionary<long, T>();

        public T Find(long id)
        {
            return _entries[id];
        }

        public void Insert(T entry)
        {
            _entries.Add(entry.Id, entry);
        }
    }
}
