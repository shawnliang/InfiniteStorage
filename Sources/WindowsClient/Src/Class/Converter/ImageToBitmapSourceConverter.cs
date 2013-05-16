using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Waveface.Client
{
	public class ImageToBitmapSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
	object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;

			var bitmapImage = default(BitmapImage);
			var img = value as System.Drawing.Image;
			using (var memory = new MemoryStream())
			{
				img.Save(memory, ImageFormat.Png);
				memory.Position = 0;
				bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
			}
			return bitmapImage;
		}

		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
