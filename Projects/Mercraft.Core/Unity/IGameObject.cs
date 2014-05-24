namespace Mercraft.Core.Unity
{
    /// <summary>
    ///     Should hold Unity specific GameObject class.
    ///     Actually, this is workaround which allows to use classes
    ///     outside Unity environment
    /// </summary>
    public interface IGameObject
    {
        T GetComponent<T>();
    }
}