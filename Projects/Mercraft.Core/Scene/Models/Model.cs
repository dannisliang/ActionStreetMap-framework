
using System.Collections.Generic;

namespace Mercraft.Core.Scene.Models
{
    public abstract class Model
    {
        public string Id { get; set; }
        public ICollection<KeyValuePair<string, string>> Tags { get; set; }


        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        public override string ToString()
        {
            string tags = "{no tags}";
            if (this.Tags != null && this.Tags.Count > 0)
            {
                tags = "tags:{";
                foreach (var tag in Tags)
                {
                    tags += string.Format("{0}:{1},", tag.Key, tag.Value);
                }
                tags += "}";
            }
            return string.Format("{0}[{1}]{2}", GetType().Name, Id, tags);
        }
    }
}
