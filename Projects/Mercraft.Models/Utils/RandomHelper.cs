
namespace Mercraft.Models.Utils
{
    public static class RandomHelper
    {
        public static int GetIndex(long seed, int count)
        {
            return (int) seed % count;
        }
    }
}
