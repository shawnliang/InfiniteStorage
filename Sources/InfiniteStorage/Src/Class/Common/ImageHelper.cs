using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Wammer.Utility
{
	public enum ExifOrientations : byte
	{
		Unknown = 0,
		TopLeft = 1,
		TopRight = 2,
		BottomRight = 3,
		BottomLeft = 4,
		LeftTop = 5,
		RightTop = 6,
		RightBottom = 7,
		LeftBottom = 8,
	}


	public static class ImageHelper
	{
		private const int OrientationId = 0x0112;

		public static ImageCodecInfo JpegCodec;
		public static ImageCodecInfo PngCodec;
		public static ImageCodecInfo GifCodec;

		static ImageHelper()
		{
			var codecs = ImageCodecInfo.GetImageEncoders();
			foreach (var codec in codecs)
			{
				switch (codec.MimeType.ToLower())
				{
					case "image/gif":
						GifCodec = codec;
						break;
					case "image/jpeg":
						JpegCodec = codec;
						break;
					case "image/png":
						PngCodec = codec;
						break;
				}
			}
		}

		public static Image ScaleBasedOnLongSide(Bitmap original, int sideLength)
		{
			var ratio = sideLength / (float)((original.Width > original.Height) ? original.Width : original.Height);


			return Scale(original, ratio);
		}

		public static Image ScaleBasedOnShortSide(Bitmap original, int sideLength)
		{
			var ratio = sideLength / (float)((original.Width < original.Height) ? original.Width : original.Height);

			return Scale(original, ratio);
		}

		private static Image Scale(Bitmap original, float ratio)
		{
			if (ratio >= 1)
				return original;

			var scaledWidth = (int)(original.Width * ratio);
			var scaledHeight = (int)(original.Height * ratio);
			return new Bitmap(original, new Size(scaledWidth, scaledHeight));
		}

		public static Bitmap Crop(Image original, int width, int height)
		{
			var cropedImage = new Bitmap(width, height);

			using (Graphics g = Graphics.FromImage(cropedImage))
			{
				g.DrawImage(original, new Rectangle(0, 0, width, height),
							new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
			}

			return cropedImage;
		}


		public static Bitmap Crop(Image original, int x, int y, int squareSize)
		{
			var cropedImage = new Bitmap(squareSize, squareSize);

			using (Graphics g = Graphics.FromImage(cropedImage))
			{
				if (original.Width - x < squareSize)
					x = original.Width - squareSize;

				if (original.Height - y < squareSize)
					y = original.Height - y;

				g.DrawImage(original, new Rectangle(0, 0, squareSize, squareSize),
							new Rectangle(x, y, squareSize, squareSize), GraphicsUnit.Pixel);
			}

			return cropedImage;
		}

		public static int LongSizeLength(Image img)
		{
			return img.Width > img.Height ? img.Width : img.Height;
		}

		public static int ShortSizeLength(Image img)
		{
			return img.Width < img.Height ? img.Width : img.Height;
		}

		// Return the image's orientation.
		public static ExifOrientations ImageOrientation(Image img)
		{
			// Get the index of the orientation property.
			int orientation_index = Array.IndexOf(img.PropertyIdList, OrientationId);

			// If there is no such property, return Unknown.
			if (orientation_index < 0) return ExifOrientations.Unknown;

			// Return the orientation value.
			return (ExifOrientations)img.GetPropertyItem(OrientationId).Value[0];
		}

		public static void CorrectOrientation(ExifOrientations orientation, Image pic)
		{
			switch (orientation)
			{
				case ExifOrientations.TopRight:
					pic.RotateFlip(RotateFlipType.RotateNoneFlipX);
					break;
				case ExifOrientations.BottomRight:
					pic.RotateFlip(RotateFlipType.Rotate180FlipNone);
					break;
				case ExifOrientations.BottomLeft:
					pic.RotateFlip(RotateFlipType.RotateNoneFlipY);
					break;
				case ExifOrientations.LeftTop:
					pic.RotateFlip(RotateFlipType.Rotate90FlipX);
					break;
				case ExifOrientations.RightTop:
					pic.RotateFlip(RotateFlipType.Rotate90FlipNone);
					break;
				case ExifOrientations.RightBottom:
					pic.RotateFlip(RotateFlipType.Rotate270FlipX);
					break;
				case ExifOrientations.LeftBottom:
					pic.RotateFlip(RotateFlipType.Rotate270FlipNone);
					break;
				default:
					return;
			}

			try
			{
				pic.RemovePropertyItem(OrientationId);
			}
			catch
			{
			}
		}

		public static Size GetImageSize(ArraySegment<byte> imageRawData)
		{
			try
			{
				using (var m = new MemoryStream(imageRawData.Array, imageRawData.Offset, imageRawData.Count))
				{
					var decoder = BitmapDecoder.Create(m, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
					var frame = decoder.Frames[0];

					return new Size(frame.PixelWidth, frame.PixelHeight);
				}
			}
			catch (Exception e)
			{
				throw new Exception("Seems the attachment is not a valid image", e);
			}
		}

		public static Size GetImageSize(Stream imageStream)
		{
			try
			{
				var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
				var frame = decoder.Frames[0];

				return new Size(frame.PixelWidth, frame.PixelHeight);
			}
			catch (Exception e)
			{
				throw new Exception("Seems the attachment is not a valid image", e);
			}
		}
	}
}