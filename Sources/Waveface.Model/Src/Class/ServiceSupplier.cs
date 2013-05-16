using System;
using System.Collections.Generic;

namespace Waveface.Model
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ServiceSupplier : IServiceSupplier
	{
		#region Var
		private IEnumerable<IService> _services;
		#endregion


		#region Protected Property
		/// <summary>
		/// Gets the info.
		/// </summary>
		/// <value>The info.</value>
		protected abstract ServiceSupplierInfo m_Info { get; }
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the ID.
		/// </summary>
		/// <value>
		/// The ID.
		/// </value>
		public abstract String ID { get; }

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return m_Info.Name; }
		}


		/// <summary>
		/// Gets the comment.
		/// </summary>
		/// <value>The comment.</value>
		public string Comment
		{
			get { return m_Info.Comment; }
		}


		/// <summary>
		/// Gets the services.
		/// </summary>
		/// <value>The services.</value>
		public virtual IEnumerable<IService> Services
		{
			get
			{
				return _services;
			}
			protected set
			{
				_services = value;
			}
		}


		public string Description
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public object Tag { get; set; }
		#endregion


		#region Public Method
		public override string ToString()
		{
			return this.Name;
		}
		#endregion
	}
}
