#region

using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Waveface.Client
{
	public partial class RightSidePanel2 : UserControl
	{
		public event EventHandler OnAirClick;
		public event EventHandler CloudSharingClick;
		public event EventHandler DeleteButtonClick;

		#region FavoriteName

		public static readonly DependencyProperty _favoriteName = DependencyProperty.Register("FavoriteName", typeof (string), typeof (RightSidePanel2),
		                                                                                      new UIPropertyMetadata(string.Empty, OnFavoriteNameChanged));

		public string FavoriteName
		{
			get { return (string) GetValue(_favoriteName); }
			set
			{
				SetValue(_favoriteName, value);
				tbxName.Text = value;
			}
		}

		#endregion

		public RightSidePanel2()
		{
			InitializeComponent();
		}

		#region Protected Method

		protected void OnOnAirClick(EventArgs e)
		{
			if (OnAirClick == null)
				return;

			OnAirClick(this, e);
		}

		protected void OnCloudSharingClick(EventArgs e)
		{
			if (CloudSharingClick == null)
				return;

			CloudSharingClick(this, e);
		}

		protected void OnDeleteButtonClick(EventArgs e)
		{
			if (DeleteButtonClick == null)
				return;

			DeleteButtonClick(this, e);
		}

		#endregion

		private static void OnFavoriteNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (o == null)
				return;

			var control = o as RightSidePanel2;
			control.FavoriteName = (string) e.NewValue;
		}

		private void ToggleButton_Click(object sender, RoutedEventArgs e)
		{
			OnOnAirClick(EventArgs.Empty);
		}

		private void tbxName_TextChanged(object sender, TextChangedEventArgs e)
		{
			FavoriteName = tbxName.Text;
		}

		private void btnAction_Copy1_Click(object sender, RoutedEventArgs e)
		{
			OnDeleteButtonClick(EventArgs.Empty);
		}

		private void swbHomeSharing_IsOnStatusChanged(object sender, EventArgs e)
		{
			OnOnAirClick(EventArgs.Empty);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OnCloudSharingClick(EventArgs.Empty);
		}
	}
}