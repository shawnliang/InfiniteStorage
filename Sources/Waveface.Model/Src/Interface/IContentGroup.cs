using System;
using System.Collections.Generic;

namespace Waveface.Model
{
	public interface IContentGroup : IContentEntity
	{
		#region Property
		IEnumerable<IContentEntity> Contents { get; }
		int ContentCount { get; }
		#endregion

		#region Event
		event EventHandler<ContentPropertyChangeEventArgs> ContentPropertyChanged;
		#endregion
	}
}
