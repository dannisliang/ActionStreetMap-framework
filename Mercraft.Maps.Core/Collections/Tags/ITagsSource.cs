
namespace Mercraft.Maps.Core.Collections.Tags
{
	/// <summary>
	/// An abstract tags source definition.
	/// </summary>
	public interface ITagsSource
	{
		/// <summary>
		/// Tries to get the value for the given key.
		/// </summary>
		/// <returns><c>true</c>, if get value was found, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		bool TryGetValue(string key, out string value);
	}
}

