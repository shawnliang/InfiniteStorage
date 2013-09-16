using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Waveface.Client
{
	/// <summary>
	/// IntroDialog.xaml 的互動邏輯
	/// </summary>
	public partial class IntroDialog : Window
	{
		private int currentPage = 1;
		private const int MAX_PAGE = 3;

		private string cultureName;

		public IntroDialog()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			cultureName = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\BunnyHome", "Culture", "en-US");

			SetImage(1);
			prevBtn.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void SetImage(int page)
		{
			currentPage = page;
			var curImgUri = string.Format("pack://application:,,,/Resource/Intro{0}.{1}.png", currentPage, cultureName);
			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(curImgUri);
			bitmap.EndInit();
			img.Source = bitmap;
		}

		private void nextBtn_Click(object sender, RoutedEventArgs e)
		{
			if (currentPage < MAX_PAGE)
			{
				SetImage(currentPage + 1);
				prevBtn.Visibility = System.Windows.Visibility.Visible;

				if (currentPage == MAX_PAGE)
				{
					nextBtn.Content = FindResource("intro_last_step") as string;
				}
			}
			else
			{
				Close();
			}
		}

		private void prevBtn_Click(object sender, RoutedEventArgs e)
		{
			if (currentPage > 1)
			{
				SetImage(currentPage - 1);

				if (currentPage == 1)
					prevBtn.Visibility = System.Windows.Visibility.Collapsed;

				nextBtn.Content = FindResource("intro_next") as string;
			}
		}
	}
}
