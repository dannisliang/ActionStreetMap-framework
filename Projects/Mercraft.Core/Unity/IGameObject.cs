namespace Mercraft.Core.Unity
{
    /// <summary>
    /// Represents GameObject. Introduced for usage in non-Unity contexts
    /// </summary>
    public interface IGameObject
    {
        T GetComponent<T>();
    }
}
