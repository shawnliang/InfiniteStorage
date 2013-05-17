using System;
using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for LabelTag.xaml
	/// </summary>
	public partial class LabelTag : UserControl
	{
		#region Var
		public static readonly DependencyProperty _tagged = DependencyProperty.Register("Tagged", typeof(bool), typeof(LabelTag), new UIPropertyMetadata(false, new PropertyChangedCallback(OnTaggedChanged)));
		#endregion

		#region Property
		public bool Tagged
		{
			get
			{
				return (bool)GetValue(_tagged);
			}
			set
			{
				OnTagStatusChanging(EventArgs.Empty);
				SetValue(_tagged, value);
				imgSelected.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				OnTagStatusChanged(EventArgs.Empty);
			}
		}
		#endregion


		#region Event
		public event EventHandler TagStatusChanging;
		public event EventHandler TagStatusChanged;
		#endregion


		#region Constructor
		public LabelTag()
		{
			this.InitializeComponent();
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
		private static void OnTaggedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;
			var labelTag = o as LabelTag;
			labelTag.Tagged = (bool)e.NewValue;
		}

		private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			this.Tagged = !this.Tagged;
		}
		#endregion
	}
}