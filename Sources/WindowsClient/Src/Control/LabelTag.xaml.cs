#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Waveface.Client
{
	public partial class LabelTag : UserControl
	{
		#region Var

		public static readonly DependencyProperty _tagged = DependencyProperty.Register("Tagged", typeof(bool), typeof(LabelTag), new UIPropertyMetadata(false, OnTaggedChanged));
		private bool m_isMouseClick;

		#endregion

		#region Property

		public bool Tagged
		{
			get { return (bool)GetValue(_tagged); }
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
			InitializeComponent();
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

			if(labelTag.m_isMouseClick)
				return;

			labelTag.Tagged = (bool)e.NewValue;
		}

		private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			m_isMouseClick = true;

			Tagged = !Tagged;

			m_isMouseClick = false;

			e.Handled = true;
		}

		#endregion
	}
}