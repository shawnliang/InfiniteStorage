#region

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InfiniteStorage.Data;
using Waveface.ClientFramework;

#endregion

namespace Waveface.Client
{
	public partial class IntroStep4 : Window
	{
		private int num_of_star;
		private Style starredStyle;
		private Style unstarredStyle;

		public IntroStep4()
		{
			InitializeComponent();

			starredStyle = (Style) FindResource("starredStyle");
			unstarredStyle = (Style) FindResource("unstarredStyle");
		}

		private void star_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var stars = new List<Image> {star1, star2, star3, star4, star5};

			if (sender == stars[0])
				num_of_star = 1;
			else if (sender == stars[1])
				num_of_star = 2;
			else if (sender == stars[2])
				num_of_star = 3;
			else if (sender == stars[3])
				num_of_star = 4;
			else if (sender == stars[4])
				num_of_star = 5;
			else
				throw new InvalidOperationException();


			for (int i = 0; i < num_of_star; i++)
			{
				stars[i].Style = starredStyle;
			}

			for (int i = num_of_star; i < stars.Count; i++)
			{
				stars[i].Style = unstarredStyle;
			}
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			var defString = FindResource("intro_step5_your_suggestion") as String;

			if (suggestion.Text == defString)
				suggestion.Text = "";
		}

		private void suggestion_LostFocus(object sender, RoutedEventArgs e)
		{
			if (suggestion.Text.Trim().Length == 0)
				suggestion.Text = FindResource("intro_step5_your_suggestion") as String;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (num_of_star == 0)
			{
				errorText.Visibility = Visibility.Visible;
				return;
			}

			var comment = suggestion.Text;
			var api = new UserTrackApi();

			api.CallAync(
				Environment.OSVersion.ToString(),
				Assembly.GetExecutingAssembly().GetName().Version.ToString(),
				ProgramConfig.ApiBaseUri.Host.Contains("dev") ? "development" : "production",
				num_of_star,
				comment,
				"install");

			Close();
		}
	}
}