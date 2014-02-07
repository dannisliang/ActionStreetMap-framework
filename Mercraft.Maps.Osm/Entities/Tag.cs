
namespace Mercraft.Maps.Osm.Entities
{
    public struct Tag
    {
        /// <summary>
        /// Creates a new tag.
        /// </summary>
        public Tag(string key, string value)
            : this()
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>
        /// The key (or the actual tag name).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value of the tag.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns a description of this tag.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}={1}", this.Key, this.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Tag)
            {
                return this.Key == ((Tag)obj).Key &&
                    this.Value == ((Tag)obj).Value;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            if (this.Key == null && this.Value == null)
            {
                return 1501234;
            }
            else if (this.Key == null)
            {
                return 140011346 ^
                    this.Value.GetHashCode();
            }
            else if (this.Value == null)
            {
                return 103254761 ^
                    this.Key.GetHashCode();
            }
            return this.Key.GetHashCode() ^
                this.Value.GetHashCode();
        }
    }
}
