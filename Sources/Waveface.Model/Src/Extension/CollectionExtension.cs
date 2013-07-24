#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

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

		public static void RemoveRange<T>(this Collection<T> obj, IEnumerable<T> values)
		{
			foreach (var value in values)
				obj.Remove(value);
		}

		public static void RefreshTo<T>(this Collection<T> obj, IEnumerable<T> values)
		{
			var newValues = values.Except(obj);
			var expiredValues = obj.Except(values).ToArray();

			if (!newValues.Any() && !expiredValues.Any())
				return;

			obj.RemoveRange(expiredValues);
			obj.AddRange(newValues);
		}
	}
}