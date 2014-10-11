using Mercraft.Infrastructure.Formats.Json;

namespace Mercraft.Infrastructure.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represens a single element of xml
    /// </summary>
    public class ConfigElement
    {
        private readonly string _xpath;
        private JSONNode _node;
        private string _attribute;

        public ConfigElement(JSONNode node)
        {
            this._node = node;

        }

        public ConfigElement(JSONNode node, string xpath)
        {
            this._node = node;
            this._xpath = xpath;

            this.Initialize();
        }

        private void Initialize()
        {
            try
            {
                string[] paths = this._xpath.Split('/');

                JSONNode current = this._node;

                if (_xpath == "")
                    return;

                for (int i = 0; i < paths.Length; i++)
                {
                   /* if (paths[i].StartsWith("@"))
                    {
                        this._attribute = current[(paths[i].Substring(1))].Value;
                        this._node = null;
                        return;
                    }*/


                    current = current[(paths[i])];
                    if (current == null)
                        break;
                }

                this._node = current;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format("Unable to process xml. xpath:{0}\n node:{1}", _xpath, _node), ex);
            }
        }

        /// <summary>
        /// Returns the set of elements
        /// </summary>
        public IEnumerable<ConfigElement> GetElements(string xpath)
        {
            if (this.Node == null)
                return Enumerable.Empty<ConfigElement>();

            string[] paths = xpath.Split('/');
            int last = paths.Length - 1;
            JSONNode current = this.Node;
            for (int i = 0; i < last; i++)
            {
                current = current[paths[i]];
                //xpath isn't valid
                if (current == null)
                    return Enumerable.Empty<ConfigElement>();
            }

            return 
                from JSONNode node in current[paths[last]].AsArray 
                select new ConfigElement(node);
        }

        /// <summary>
        /// Returns string
        /// </summary>
        public string GetString()
        {
            if (this.IsAttribute) return this._attribute;
            if (this.IsNode) return this._node.Value;

            return null;
        }

        /// <summary>
        /// Returns int
        /// </summary>
        public int GetInt()
        {
            return int.Parse(this.GetString());
        }

        /// <summary>
        /// Returns float
        /// </summary>
        public float GetFloat()
        {
            return float.Parse(this.GetString());
        }

        /// <summary>
        /// Returns boolean
        /// </summary>
        public bool GetBool()
        {
            return bool.Parse(this.GetString());
        }

        /// <summary>
        /// Returns type
        /// </summary>
        public new Type GetType()
        {
            string typeName = this.GetString();
            return Type.GetType(typeName);
        }

        /// <summary>
        /// Returns current XElement
        /// </summary>
        public JSONNode Node
        {
            get { return this._node; }
        }

        /// <summary>
        /// Returns current XAttribute
        /// </summary>
        public string Attribute
        {
            get { return this._attribute; }
        }

        /// <summary>
        /// true if element represents attribute
        /// </summary>
        public bool IsAttribute
        {
            get { return this._attribute != null; }
        }

        /// <summary>
        /// true if element represents xml node
        /// </summary>
        public bool IsNode
        {
            get { return this._node != null; }
        }

        public bool IsEmpty
        {
            get
            {
                 return (!this.IsAttribute) && (!this.IsNode);
            }
        }
    }
}
