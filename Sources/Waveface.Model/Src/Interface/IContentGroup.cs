using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

		#region Method
		void Refresh();
		#endregion
	}
}
