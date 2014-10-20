using System;
using System.Collections.Generic;
using System.Linq;
using Mercraft.Infrastructure.Formats.Json;
using Mercraft.Infrastructure.IO;

namespace Mercraft.Infrastructure.Config
{
    /// <summary>
    ///     Represens a JSON config entry.
    /// </summary>
    public class ConfigSection : IConfigSection
    {
        private readonly ConfigElement _element;

        /// <summary>
        ///     Creates ConfigSection.
        /// </summary>
        /// <param name="element">Config element.</param>
        public ConfigSection(ConfigElement element)
        {
            _element = element;
        }

        /// <summary>
        ///     Creates ConfigSection.
        /// </summary>
        /// <param name="appConfigFileName">Config appConfig.</param>
        /// <param name="fileSystemService">File system service</param>
        public ConfigSection(string appConfigFileName, IFileSystemService fileSystemService)
        {
            var jsonStr = fileSystemService.ReadText(appConfigFileName);
            var json = JSON.Parse(jsonStr);
            _element = new ConfigElement(json);
        }

        /// <summary>
        ///     Creates ConfigSection.
        /// </summary>
        /// <param name="content">Json content</param>
        public ConfigSection(string content)
        {
            _element = new ConfigElement(JSON.Parse(content));
        }

        /// <inheritdoc />
        public IEnumerable<IConfigSection> GetSections(string xpath)
        {
            return _element.GetElements(xpath).Select(e => (new ConfigSection(e)) as IConfigSection);
        }

        /// <inheritdoc />
        public IConfigSection GetSection(string xpath)
        {
            return new ConfigSection(new ConfigElement(_element.Node, xpath));
        }

        /// <inheritdoc />
        public bool IsEmpty
        {
            get { return _element.IsEmpty; }
        }

        /// <inheritdoc />
        public string GetString(string xpath)
        {
            return new ConfigElement(_element.Node, xpath).GetString();
        }

        /// <inheritdoc />
        public int GetInt(string xpath)
        {
            return new ConfigElement(_element.Node, xpath).GetInt();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public float GetFloat(string xpath)
        {
            return new ConfigElement(_element.Node, xpath).GetFloat();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool GetBool(string xpath)
        {
            return new ConfigElement(_element.Node, xpath).GetBool();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public Type GetType(string xpath)
        {
            return (new ConfigElement(_element.Node, xpath)).GetType();
        }

        /// <inheritdoc />
        public T GetInstance<T>(string xpath)
        {
            return (T) Activator.CreateInstance(GetType(xpath));
        }

        /// <inheritdoc />
        public T GetInstance<T>(string xpath, params object[] args)
        {
            return (T) Activator.CreateInstance(GetType(xpath), args);
        }
    }
}