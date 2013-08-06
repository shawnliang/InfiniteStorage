#region

using System;
using System.Windows.Controls;

#endregion

namespace Waveface.Client
{
	public partial class ContentItem : UserControl
	{
		#region Property

		public bool Tagged
		{
			get { return ltTag.Tagged; }
			set { ltTag.Tagged = value; }
		}

		#endregion

		#region Event

		public event EventHandler TagStatusChanged;

		#endregion

		#region Constructor

		public ContentItem()
		{
			InitializeComponent();

			ltTag.TagStatusChanged += ltTag_TagStatusChanged;
		}

		#endregion

		#region Protected Method
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

		private void ltTag_TagStatusChanged(object sender, EventArgs e)
		{
			OnTagStatusChanged(EventArgs.Empty);
		}
		#endregion
	}
}