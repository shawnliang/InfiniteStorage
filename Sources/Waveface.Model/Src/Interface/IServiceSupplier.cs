using System.Collections.Generic;

namespace Waveface.Model
{
	/// <summary>
	/// 
	/// </summary>
	public interface IServiceSupplier
	{
		#region Property
		/// <summary>
		/// Gets the ID.
		/// </summary>
		/// <value>
		/// The ID.
		/// </value>
		string ID { get; }


		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		string Name { get; }


		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		string Description { get; }

		//Profile Profile { get; set; }
		/// <summary>
		/// Gets the services.
		/// </summary>
		/// <value>The services.</value>
		IEnumerable<IService> Services { get; }
		#endregion
	}
}
