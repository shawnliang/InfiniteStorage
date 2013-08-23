using System;
using System.Globalization;
using System.Windows.Data;

namespace Waveface.Client
{
	public class ListBoxToSelectionTextConvertor : IMultiValueConverter
	{
		public string SelectionTemplate { get; set; }
		public string NoSelectionTemplate { get; set; }
		public string NoItemTemplate { get; set; }

		public ListBoxToSelectionTextConvertor()
		{
			NoItemTemplate = NoSelectionTemplate = SelectionTemplate = "";
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var total = (int)values[0];
				var selected = (int)values[1];

				if (total == 0)
					return NoItemTemplate;
				else if (selected > 0)
					return string.Format(SelectionTemplate, selected);
				else
					return string.Format(NoSelectionTemplate, total);
			}
			catch
			{
				return null;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
