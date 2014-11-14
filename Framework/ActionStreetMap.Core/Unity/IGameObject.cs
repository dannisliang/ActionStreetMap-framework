namespace ActionStreetMap.Core.Unity
{
    /// <summary>
    ///     Should hold Unity specific GameObject class. Actually, this is workaround which allows to use classes outside Unity environment
    /// </summary>
    public interface IGameObject
    {
        /// <summary>
        ///     Gets component of given type.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <returns>Component.</returns>
        T GetComponent<T>();

        /// <summary>
        ///     Gets or sets name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Sets parent game object.
        /// </summary>
        IGameObject Parent { set; }
    }
}