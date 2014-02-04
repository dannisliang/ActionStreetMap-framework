
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Mercraft.Maps.Core.Collections.Tags
{
    /// <summary>
    /// Represents a generic tags collection.
    /// </summary>
    public abstract class TagsCollectionBase : IEnumerable<Tag>, IEnumerable<KeyValuePair<string, string>>, ITagsSource
    {
        /// <summary>
        /// Returns the number of tags in this collection.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Adds a key-value pair to this tags collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void Add(string key, string value);

        /// <summary>
        /// Adds a tag.
        /// </summary>
        /// <param name="tag"></param>
        public abstract void Add(Tag tag);

        /// <summary>
        /// Adds all tags from the given collection.
        /// </summary>
        /// <param name="tagsCollection"></param>
        public void Add(TagsCollectionBase tagsCollection)
        {
            foreach (Tag tag in tagsCollection)
            {
                this.Add(tag);
            }
        }

        /// <summary>
        /// Adds the tags or replaces the existing value if any.
        /// </summary>
        /// <param name="tagsCollection"></param>
        public void AddOrReplace(TagsCollectionBase tagsCollection)
        {
            foreach (Tag tag in tagsCollection)
            {
                this.AddOrReplace(tag);
            }
        }

        /// <summary>
        /// Adds a tag or replace the existing value if any.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void AddOrReplace(string key, string value);

        /// <summary>
        /// Adds a tag or replace the existing value if any.
        /// </summary>
        /// <param name="tag"></param>
        public abstract void AddOrReplace(Tag tag);

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool ContainsKey(string key);

        /// <summary>
        /// Returns true if the given tag exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool TryGetValue(string key, out string value);

        /// <summary>
        /// Returns true if the given tags exists with the given value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool ContainsKeyValue(string key, string value);

        /// <summary>
        /// Returns the value associated with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Throws a KeyNotFoundException when the key does not exists. Use TryGetValue.</returns>
        public virtual string this[string key]
        {
            get
            {
                string value;
                if (this.TryGetValue(key, out value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                this.AddOrReplace(key, value);
            }
        }

        /// <summary>
        /// Returns a parsed numeric value if available.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double? GetNumericValue(string key)
        {
            string value;
            if (this.TryGetValue(key, out value))
            {
                double numericValue;
                if (double.TryParse(value, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture,
                                    out numericValue))
                {
                    return numericValue;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes all tags with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract bool RemoveKey(string key);

        /// <summary>
        /// Removes the given tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public virtual bool RemoveKeyValue(Tag tag)
        {
            return this.RemoveKeyValue(tag.Key, tag.Value);
        }

        /// <summary>
        /// Removes all tags with the given key-values.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool RemoveKeyValue(string key, string value);

        /// <summary>
        /// Clears all tags.
        /// </summary>
        public abstract void Clear();

        #region IEnumerable<Tag>

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator<Tag> GetEnumerator();

        /// <summary>
        /// Returns the enumerator for this enumerable.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Returns the enumerable for KeyValuePairs.
        /// </summary>
        /// <returns></returns>
        #region IEnumerable<KeyValuePair<string, string>>

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// A KeyValuePair enumerator wrapper around a IEnumerator tag enumerator.
        /// </summary>
        private class KeyValuePairEnumerator : IEnumerator<KeyValuePair<string, string>>
        {
            private IEnumerator<Tag> _tagEnumerator;

            public KeyValuePairEnumerator(IEnumerator<Tag> tagEnumerator)
            {
                _tagEnumerator = tagEnumerator;
            }

            public KeyValuePair<string, string> Current
            {
                get { return new KeyValuePair<string,string>(_tagEnumerator.Current.Key, _tagEnumerator.Current.Value); }
            }

            public void Dispose()
            {
                _tagEnumerator.Dispose();
            }

            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                return _tagEnumerator.MoveNext();
            }

            public void Reset()
            {
                _tagEnumerator.Reset();
            }
        }

        #endregion
    }
}