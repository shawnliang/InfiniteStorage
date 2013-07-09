using System;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for ContentItem.xaml
	/// </summary>
	public partial class ContentItem : UserControl
	{
		#region Property
		public bool Tagged
		{
			get
			{
				return ltTag.Tagged;
			}
			set
			{
				ltTag.Tagged = value;
			}
		}
		#endregion


		#region Event
		public event EventHandler TagStatusChanging;
		public event EventHandler TagStatusChanged;
		#endregion


		#region Constructor
		public ContentItem()
		{
			this.InitializeComponent();
			this.ltTag.TagStatusChanging += ltTag_TagStatusChanging;
			this.ltTag.TagStatusChanged += ltTag_TagStatusChanged;
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:TagStatusChanging" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OnTagStatusChanging(EventArgs e)
		{
			if (TagStatusChanging == null)
				return;
			TagStatusChanging(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:TagStatusChanged" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OnTagStatusChanged(EventArgs e)
		{
			if (TagStatusChanged == null)
				return;
			TagStatusChanged(this, e);
		}
		#endregion



		#region Event Process
		void ltTag_TagStatusChanged(object sender, EventArgs e)
		{
			OnTagStatusChanged(EventArgs.Empty);
		}

		void ltTag_TagStatusChanging(object sender, EventArgs e)
		{
			OnTagStatusChanging(EventArgs.Empty);
		}
		#endregion
	}
}