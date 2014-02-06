using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercraft.Maps.Osm.Entities
{
    public struct Tag
    {
        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
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
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", this.Key, this.Value);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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
        /// <returns></returns>
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
