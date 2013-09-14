using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Waveface.Client
{
	/// <summary>
	/// IntroStep5.xaml 的互動邏輯
	/// </summary>
	public partial class IntroStep4 : Window
	{
		private int num_of_star = 0;
		private Style starredStyle;
		private Style unstarredStyle;

		public IntroStep4()
		{
			InitializeComponent();

			starredStyle = (Style)FindResource("starredStyle");
			unstarredStyle = (Style)FindResource("unstarredStyle");
		}

		private void star_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var panel = LogicalTreeHelper.GetParent(sender as DependencyObject) as UniformGrid;

			var stars = new List<Image> {};

			foreach (Image star in LogicalTreeHelper.GetChildren(panel))
			{
				stars.Add(star);
			}
			

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
			TextBox suggestion = findSuggestionBox(sender);

			var defString = FindResource("intro_step5_your_suggestion") as String;

			if (suggestion.Text == defString)
				suggestion.Text = "";
		}

		private static TextBox findSuggestionBox(object sender)
		{
			var panel = LogicalTreeHelper.GetParent(sender as DependencyObject) as Panel;
			TextBox suggestion = null;

			foreach (var child in LogicalTreeHelper.GetChildren(panel))
			{
				if (child is TextBox)
				{
					suggestion = child as TextBox;
					break;
				}
			}
			return suggestion;
		}

		private void suggestion_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox suggestion = findSuggestionBox(sender);

			if (suggestion.Text.Trim().Length == 0)
				suggestion.Text = FindResource("intro_step5_your_suggestion") as String;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (num_of_star == 0)
			{
				Panel parent = LogicalTreeHelper.GetParent(sender as DependencyObject) as Panel;
				foreach (var child in LogicalTreeHelper.GetChildren(parent))
				{
					if (child is TextBlock)
					{
						(child as TextBlock).Visibility = System.Windows.Visibility.Visible;
					}
				}

				return;
			}

			Close();
		}
	}
}
