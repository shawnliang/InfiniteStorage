using System;
using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for RightSidePanel2.xaml
	/// </summary>
	public partial class RightSidePanel2 : UserControl
	{
		#region Var
		public static readonly DependencyProperty _favoriteName = DependencyProperty.Register("FavoriteName", typeof(string), typeof(RightSidePanel2), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnFavoriteNameChanged)));
		#endregion

		#region Property
		public string FavoriteName
		{
			get
			{
				return (string)GetValue(_favoriteName);
			}
			set
			{
				SetValue(_favoriteName, value);
				tbxName.Text = value;
			}
		}
		#endregion

		#region Event
		public event EventHandler OnAirClick;
		public event EventHandler CloudSharingClick;
		public event EventHandler DeleteButtonClick;
		#endregion


		public RightSidePanel2()
		{
			this.InitializeComponent();
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
			control.FavoriteName = (string)e.NewValue;
		}

		private void ToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OnOnAirClick(EventArgs.Empty);
		}

		private void tbxName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			FavoriteName = tbxName.Text;
		}

		private void btnAction_Copy1_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OnDeleteButtonClick(EventArgs.Empty);
		}

		private void swbHomeSharing_IsOnStatusChanged(object sender, System.EventArgs e)
		{
			OnOnAirClick(EventArgs.Empty);
		}

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			OnCloudSharingClick(EventArgs.Empty);
		}
	}
}