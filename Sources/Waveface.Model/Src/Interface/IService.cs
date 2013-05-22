using System;
using System.Collections.Generic;

namespace Waveface.Model
{
	/// <summary>
	/// 
	/// </summary>
	public interface IService
	{
		#region Property
		string Name { get; }

		/// <summary>
		/// Gets the supplier.
		/// </summary>
		/// <value>The supplier.</value>
		IServiceSupplier Supplier { get; }

		/// <summary>
		/// Gets or sets the contents.
		/// </summary>
		/// <value>The contents.</value>
		IEnumerable<IContentEntity> Contents { get; }

		string Description { get; }
		#endregion

		#region Event
		event EventHandler<ContentPropertyChangeEventArgs> ContentPropertyChanged;
		#endregion
	}
}
