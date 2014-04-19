namespace Mercraft.Infrastructure.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represens a config entry
    /// </summary>
    public class ConfigSection : IConfigSection
    {
        private readonly ConfigElement _element;
        public ConfigSection(ConfigElement element)
        {
            this._element = element;
        }

        /// <summary>
        /// Returns the set of ConfigSections
        /// </summary>
        public IEnumerable<IConfigSection> GetSections(string xpath)
        {
            return this._element.GetElements(xpath).Select(e => (new ConfigSection(e)) as IConfigSection);
        }

        /// <summary>
        /// Returns ConfigSection
        /// </summary>
        public IConfigSection GetSection(string xpath)
        {
            return new ConfigSection(new ConfigElement(this._element.Node, xpath));
        }

        public bool IsEmpty
        {
            get { return this._element.IsEmpty; }
        }

        /// <summary>
        /// Returns string
        /// </summary>
        public string GetString(string xpath)
        {
            return new ConfigElement(this._element.Node, xpath).GetString();
        }

        /// <summary>
        /// Returns int
        /// </summary>
        public int GetInt(string xpath)
        {
            return new ConfigElement(this._element.Node, xpath).GetInt();
        }

        /// <summary>
        /// Returns int
        /// </summary>
        public int GetInt(string xpath, int defaultValue)
        {
            try
            {
                return this.GetInt(xpath);
            }
            catch
            {
                return defaultValue;
            }
        }

        public float GetFloat(string xpath)
        {
            return new ConfigElement(this._element.Node, xpath).GetFloat();
        }

        public float GetFloat(string xpath, float defaultValue)
        {
            try
            {
                return this.GetFloat(xpath);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns bool
        /// </summary>
        public bool GetBool(string xpath)
        {
            return new ConfigElement(this._element.Node, xpath).GetBool();
        }

        public bool GetBool(string xpath, bool defaultValue)
        {
            try
            {
                return this.GetBool(xpath);
            }
            catch
            {
                return defaultValue;
            }
        }
       
        /// <summary>
        /// Returns type object
        /// </summary>
        public Type GetType(string xpath)
        {
            return (new ConfigElement(this._element.Node, xpath)).GetType();
        }


        /// <summary>
        /// Returns the instance of T
        /// </summary>
        public T GetInstance<T>(string xpath)
        {
            return (T)Activator.CreateInstance(this.GetType(xpath));
        }

        /// <summary>
        /// Returns the instance of T
        /// </summary>
        public T GetInstance<T>(string xpath, params object[] args)
        {
            return (T)Activator.CreateInstance(this.GetType(xpath), args);
        }

    }
}
