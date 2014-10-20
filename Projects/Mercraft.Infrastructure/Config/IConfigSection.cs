using System;
using System.Collections.Generic;

namespace Mercraft.Infrastructure.Config
{
    /// <summary>
    ///     Represens a config entry.
    /// </summary>
    public interface IConfigSection
    {
        /// <summary>
        ///     Returns the set of ConfigSections.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        IEnumerable<IConfigSection> GetSections(string xpath);

        /// <summary>
        ///     Returns ConfigSection.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        IConfigSection GetSection(string xpath);

        /// <summary>
        ///     True if node is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        ///     Returns string.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        string GetString(string xpath);

        /// <summary>
        ///     Returns int.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        int GetInt(string xpath);

        /// <summary>
        ///     Returns int.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        int GetInt(string xpath, int defaultValue);

        /// <summary>
        ///     Returns float.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        float GetFloat(string xpath);

        /// <summary>
        ///     Returns float.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        float GetFloat(string xpath, float defaultValue);

        /// <summary>
        ///     Returns bool.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        bool GetBool(string xpath);

        /// <summary>
        ///     Returns bool.
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        bool GetBool(string xpath, bool defaultValue);

        /// <summary>
        ///     Returns type object.
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        Type GetType(string xpath);

        /// <summary>
        ///     Returns the instance of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xpath"></param>
        /// <returns></returns>
        T GetInstance<T>(string xpath);

        /// <summary>
        ///     Returns the instance of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xpath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        T GetInstance<T>(string xpath, params object[] args);
    }
}