
namespace Mercraft.Infrastructure.Primitives
{
    public struct Range<T>
    {
        public T Minimum { get; set; }
        public T Maximum { get; set; }

        public Range(T minimum, T maximum)
            : this()
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}
