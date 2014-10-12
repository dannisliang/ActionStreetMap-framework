using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Infrastructure.Formats.Json;
using Mercraft.Infrastructure.IO;

namespace Mercraft.Infrastructure.Config
{
    /// <summary>
    ///     Represens a config entry
    /// </summary>
    public class ConfigSection : IConfigSection
    {
        private readonly ConfigElement _element;

        public ConfigSection(ConfigElement element)
        {
            _element = element;
        }

        public ConfigSection(string appConfigFileName, IFileSystemService fileSystemService)
        {
            var jsonStr = fileSystemService.ReadText(appConfigFileName);
            var json = JSON.Parse(jsonStr);
            _element = new ConfigElement(json);
        }

        public ConfigSection(string content)
        {
            _element = new ConfigElement(JSON.Parse(content));
        }

        /// <summary>
        ///     Returns the set of ConfigSections
        /// </summary>
        public IEnumerable<IConfigSection> GetSections(string xpath)
        {
            return _element.GetElements(xpath).Select(e => (new ConfigSection(e)) as IConfigSection);
        }

        /// <summary>
        ///     Returns ConfigSection
        /// </summary>
        public IConfigSection GetSection(string xpath)
        {
            return new ConfigSection(new ConfigElement(_element.Node, xpath));
        }

        public bool IsEmpty
        {
            get { return _element.IsEmpty; }
        }

        /// <summary>
        ///     Returns string
        /// </summary>
        public string GetString(string xpath)
        {
            return new ConfigElement(_element.Node, xpath).GetString();
        }

        /// <summary>
        ///     Returns int
        /// </summary>
        public int GetInt(string xpath)
        {
            return new ConfigElement(_element.Node, xpath).GetInt();
        }

        /// <summary>
        ///     Returns int
        /// </summary>
        public int GetInt(string xpath, int defaultValue)
        {
            try
            {
                return GetInt(xpath);
            }
            catch
            {
                return defaultValue;
            }
        }

        public float GetFloat(string xpath)
        {
            return new ConfigElement(_element.Node, xpath).GetFloat();
        }

        public float GetFloat(string xpath, float defaultValue)
        {
            try
            {
                return GetFloat(xpath);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        ///     Returns bool
        /// </summary>
        public bool GetBool(string xpath)
        {
            return new ConfigElement(_element.Node, xpath).GetBool();
        }

        public bool GetBool(string xpath, bool defaultValue)
        {
            try
            {
                return GetBool(xpath);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        ///     Returns type object
        /// </summary>
        public Type GetType(string xpath)
        {
            return (new ConfigElement(_element.Node, xpath)).GetType();
        }


        /// <summary>
        ///     Returns the instance of T
        /// </summary>
        public T GetInstance<T>(string xpath)
        {
            return (T) Activator.CreateInstance(GetType(xpath));
        }

        /// <summary>
        ///     Returns the instance of T
        /// </summary>
        public T GetInstance<T>(string xpath, params object[] args)
        {
            return (T) Activator.CreateInstance(GetType(xpath), args);
        }
    }
}