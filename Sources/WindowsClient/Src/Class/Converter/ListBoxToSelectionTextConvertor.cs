using System;
using System.Globalization;
using System.Windows.Data;

namespace Waveface.Client
{
	public class ListBoxToSelectionTextConvertor : IMultiValueConverter
	{
		public String SelectionTemplate { get; set; }
		public String NoSelectionTemplate { get; set; }
		public String NoItemTemplate { get; set; }

		public ListBoxToSelectionTextConvertor()
		{
			NoItemTemplate = NoSelectionTemplate = SelectionTemplate = "";
		}

		public Object Convert(object[] values, Type targetType, Object parameter, CultureInfo culture)
		{
			try
			{
				var total = (Int32)values[0];
				var selected = (Int32)values[1];

				if (total == 0)
					return NoItemTemplate;
				else if (selected > 0)
					return String.Format(SelectionTemplate, selected);
				else
					return String.Format(NoSelectionTemplate, total);
			}
			catch
			{
				return null;
			}
		}

		public Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
