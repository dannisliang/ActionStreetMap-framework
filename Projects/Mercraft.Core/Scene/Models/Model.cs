
using System.Collections.Generic;

namespace Mercraft.Core.Scene.Models
{
    public abstract class Model
    {
        public string Id { get; set; }

        public ICollection<KeyValuePair<string, string>> Tags { get; set; }
    }
}
