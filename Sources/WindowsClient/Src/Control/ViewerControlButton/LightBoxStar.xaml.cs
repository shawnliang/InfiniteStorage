#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class LightBoxStar : UserControl
	{
		#region Var

		public static readonly DependencyProperty _tagged = DependencyProperty.Register("Tagged", typeof (Boolean), typeof (LightBoxStar), new UIPropertyMetadata(false, OnTaggedChanged));

		#endregion

		#region Property

		public Boolean Tagged
		{
			get { return (Boolean) GetValue(_tagged); }
			set { SetValue(_tagged, value); }
		}

		#endregion

		#region Event

		public event EventHandler TagStatusChanged;

		#endregion

		#region Constructor

		public LightBoxStar()
		{
			InitializeComponent();
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

		private static void OnTaggedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var labelTag = o as LightBoxStar;
			labelTag.OnTagStatusChanged(EventArgs.Empty);
		}

		private void UserControl_PreviewMouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
		{
			Tagged = !Tagged;
			e.Handled = true;
		}

		#endregion
	}
}