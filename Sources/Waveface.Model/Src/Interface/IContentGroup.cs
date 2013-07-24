#region

using System;
using System.Collections.ObjectModel;

#endregion

namespace Waveface.Model
{
	public interface IContentGroup : IContentEntity
	{
		#region Property

		ReadOnlyObservableCollection<IContentEntity> Contents { get; }

		int ContentCount { get; }

		#endregion

		#region Event

		event EventHandler<ContentPropertyChangeEventArgs> ContentPropertyChanged;

		#endregion
	}
}