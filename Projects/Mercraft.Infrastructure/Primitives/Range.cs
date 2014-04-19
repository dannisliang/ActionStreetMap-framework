
namespace Mercraft.Infrastructure.Primitives
{
    public struct Range<T>
    {
        public T Min { get; set; }
        public T Max { get; set; }

        public Range(T min, T max)
            : this()
        {
            Min = min;
            Max = max;
        }
    }
}
