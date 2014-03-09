
namespace Mercraft.Core.MapCss.Domain.Selectors
{
    public abstract class Selector
    {
        public string Tag { get; set; }
        public string Value { get; set; }
        public string Operation { get; set; }

        public void Accept()
        {
            // TODO
        }
    }
}
