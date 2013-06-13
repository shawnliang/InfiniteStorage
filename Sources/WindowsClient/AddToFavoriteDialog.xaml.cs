using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for AddToFavoriteDialog.xaml
	/// </summary>
	public partial class AddToFavoriteDialog : Window
	{
		public IEnumerable FavoriteItemSource
		{
			get
			{
				return cbxFavoriteName.ItemsSource;
			}
			set
			{
				cbxFavoriteName.ItemsSource = value;

				if (cbxFavoriteName.HasItems)
					cbxFavoriteName.SelectedIndex = 0;
			}
		}

		public object SelectedFavorite 
		{
			get
			{
				return cbxFavoriteName.SelectedItem;
			}
			set
			{
				cbxFavoriteName.SelectedItem = value;
			}
		}
		
		public AddToFavoriteDialog()
		{
			this.InitializeComponent();
			
			// Insert code required on object creation below this point.
		}
		
		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
				this.DialogResult = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			cbxFavoriteName.Focus();
		}
	}
}