using System;
namespace Mercraft.Maps.Osm.Extensions
{
    public static class ConverterProvider
    {
        public static Func<string, int> IntConverter = (v) =>
        {
            int @value;
            Int32.TryParse(v, out @value);
            return @value;
        };
    }
}
