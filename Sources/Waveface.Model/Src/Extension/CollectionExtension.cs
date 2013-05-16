using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Waveface.Model
{
	public static class CollectionExtension
	{
		/// <summary>
		/// Adds the range.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="values">The values.</param>
		public static void AddRange<T>(this Collection<T> obj, IEnumerable<T> values)
		{
			foreach (var value in values)
				obj.Add(value);
		}
	}
}
