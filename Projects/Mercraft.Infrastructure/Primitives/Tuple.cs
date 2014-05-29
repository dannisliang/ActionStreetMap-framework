using System;

namespace Mercraft.Infrastructure.Primitives
{
    /// <summary>
    /// Tuple implementation
    /// </summary>
    public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((Tuple<T1, T2>)obj);
        }

        public bool Equals(Tuple<T1, T2> other)
        {
            return other.Item1.Equals(Item1) && other.Item2.Equals(Item2);
        }
    } 
}
