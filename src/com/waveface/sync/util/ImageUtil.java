package com.waveface.sync.util;


import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;

import android.content.Context;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.Bitmap.Config;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Matrix;
import android.graphics.Paint;
import android.graphics.PorterDuff.Mode;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.ColorDrawable;
import android.graphics.drawable.Drawable;
import android.graphics.drawable.TransitionDrawable;
import android.graphics.PorterDuffXfermode;
import android.graphics.Rect;
import android.graphics.RectF;
import android.media.ExifInterface;
import android.os.Environment;
import android.provider.MediaStore;
import android.text.TextUtils;
import android.widget.ImageView;


public class ImageUtil {
	private static final String TAG = "ImageUtil";
	private static final int OPTIONS_NONE = 0x0;
	private static final int OPTIONS_SCALE_UP = 0x1;
	public static final int OPTIONS_RECYCLE_INPUT = 0x2;


	/**
	 *
	 * @param strImageName
	 *            real file path
	 * @param imageSampleSize
	 * @return
	 */
	public static Bitmap getBitmapFromFile(String strImageName,
			int imageSampleSize) {
		BitmapFactory.Options options = new BitmapFactory.Options();
		options.inSampleSize = imageSampleSize;
		options.inDither = false;
		String photo_orientation = "1";
		try {
			ExifInterface exif = new ExifInterface(strImageName);
			photo_orientation = exif
					.getAttribute(ExifInterface.TAG_ORIENTATION);
		} catch (FileNotFoundException e) {
			e.printStackTrace();
			Log.d(TAG, e.getLocalizedMessage());
		} catch (IOException e) {
			e.printStackTrace();
			Log.d(TAG, e.getLocalizedMessage());
		}
		Bitmap tempMap = BitmapFactory.decodeFile(strImageName, options);

		int height = tempMap.getHeight();
		int width = tempMap.getWidth();

		// createa matrix for the manipulation
		Matrix matrix = new Matrix();

		if (photo_orientation.equals("6"))// 直拍
			matrix.postRotate(90);

		// recreate the new Bitmap
		return Bitmap.createBitmap(tempMap, 0, 0, width, height, matrix, true);

	}

	public static boolean StoreImage(Bitmap bm, String fileName) {
		boolean isExisted = false;
		try {
			File f = new File(fileName);
			FileOutputStream out = new FileOutputStream(f);
			bm.compress(Bitmap.CompressFormat.PNG, 100, out);
			// bm.recycle();
			out.flush();
			out.close();
			if (f.exists()) {
				isExisted = true;
			}

		} catch (FileNotFoundException e) {
			e.printStackTrace();
			Log.e(TAG, e.getMessage());
		} catch (IOException e) {
			e.printStackTrace();
			Log.e(TAG, e.getMessage());
		} catch (Exception e) {
			e.printStackTrace();
			Log.e(TAG, e.getMessage());
		}
		return isExisted;
	}

	public static Bitmap getRoundedCornerBitmap(Bitmap bitmap, int pixels) {
		Bitmap output = Bitmap.createBitmap(bitmap.getWidth(),
				bitmap.getHeight(), Config.ARGB_8888);
		Canvas canvas = new Canvas(output);

		final int color = 0xff424242;
		final Paint paint = new Paint();
		final Rect rect = new Rect(0, 0, bitmap.getWidth(), bitmap.getHeight());
		final RectF rectF = new RectF(rect);
		final float roundPx = pixels;

		paint.setAntiAlias(true);
		canvas.drawARGB(0, 0, 0, 0);
		paint.setColor(color);
		canvas.drawRoundRect(rectF, roundPx, roundPx, paint);

		paint.setXfermode(new PorterDuffXfermode(Mode.SRC_IN));
		canvas.drawBitmap(bitmap, rect, rect, paint);

		return output;
	}

	private static Bitmap transform(Matrix scaler, Bitmap source,
			int targetWidth, int targetHeight, int options) {
		boolean scaleUp = (options & OPTIONS_SCALE_UP) != 0;
		boolean recycle = (options & OPTIONS_RECYCLE_INPUT) != 0;

		int deltaX = source.getWidth() - targetWidth;
		int deltaY = source.getHeight() - targetHeight;
		if (!scaleUp && (deltaX < 0 || deltaY < 0)) {
			/*
			 * In this case the bitmap is smaller, at least in one dimension,
			 * than the target. Transform it by placing as much of the image as
			 * possible into the target and leaving the top/bottom or left/right
			 * (or both) black.
			 */
			Bitmap b2 = Bitmap.createBitmap(targetWidth, targetHeight,
					Bitmap.Config.ARGB_8888);
			Canvas c = new Canvas(b2);

			int deltaXHalf = Math.max(0, deltaX / 2);
			int deltaYHalf = Math.max(0, deltaY / 2);
			Rect src = new Rect(deltaXHalf, deltaYHalf, deltaXHalf
					+ Math.min(targetWidth, source.getWidth()), deltaYHalf
					+ Math.min(targetHeight, source.getHeight()));
			int dstX = (targetWidth - src.width()) / 2;
			int dstY = (targetHeight - src.height()) / 2;
			Rect dst = new Rect(dstX, dstY, targetWidth - dstX, targetHeight
					- dstY);
			c.drawBitmap(source, src, dst, null);
			if (recycle) {
				source.recycle();
			}
			return b2;
		}
		float bitmapWidthF = source.getWidth();
		float bitmapHeightF = source.getHeight();

		float bitmapAspect = bitmapWidthF / bitmapHeightF;
		float viewAspect = (float) targetWidth / targetHeight;

		if (bitmapAspect > viewAspect) {
			float scale = targetHeight / bitmapHeightF;
			if (scale < .9F || scale > 1F) {
				scaler.setScale(scale, scale);
			} else {
				scaler = null;
			}
		} else {
			float scale = targetWidth / bitmapWidthF;
			if (scale < .9F || scale > 1F) {
				scaler.setScale(scale, scale);
			} else {
				scaler = null;
			}
		}

		Bitmap b1;
		if (scaler != null) {
			// this is used for minithumb and crop, so we want to filter here.
			b1 = Bitmap.createBitmap(source, 0, 0, source.getWidth(),
					source.getHeight(), scaler, true);
		} else {
			b1 = source;
		}

		if (recycle && b1 != source) {
			source.recycle();
		}

		int dx1 = Math.max(0, b1.getWidth() - targetWidth);
		int dy1 = Math.max(0, b1.getHeight() - targetHeight);

		Bitmap b2 = Bitmap.createBitmap(b1, dx1 / 2, dy1 / 2, targetWidth,
				targetHeight);

		if (b2 != b1) {
			if (recycle || b1 != source) {
				b1.recycle();
			}
		}
		return b2;
	}

	public static Bitmap extractThumbnail(Bitmap source, int width, int height,
			int options) {
		if (source == null) {
			return null;
		}

		float scale;
		if (source.getWidth() < source.getHeight()) {
			scale = width / (float) source.getWidth();
		} else {
			scale = height / (float) source.getHeight();
		}
		Matrix matrix = new Matrix();
		matrix.setScale(scale, scale);
		Bitmap thumbnail = transform(matrix, source, width, height,
				OPTIONS_SCALE_UP | options);
		return thumbnail;
	}

	public static Bitmap extractThumbnail(Bitmap source, int width, int height) {
		return extractThumbnail(source, width, height, OPTIONS_NONE);
	}

	public static Bitmap getThumbnail(String filename, int width, int height) {
		BitmapFactory.Options options = new BitmapFactory.Options();
		options.outHeight = height;
		options.outWidth = width;
		options.inJustDecodeBounds = true;
		options.inDither = false;

		Matrix matrix = new Matrix();
		ExifInterface exif = null;
		String photoOrientation = "";
		try {
			exif = new ExifInterface(filename);
			photoOrientation = exif.getAttribute(ExifInterface.TAG_ORIENTATION);
			// portrait
			if (photoOrientation.equals("6")) {
				matrix.postRotate(90);
			}
		} catch (IOException e) {
			e.printStackTrace();
		}

		Bitmap bm = BitmapFactory.decodeFile(filename, options);
		options.inJustDecodeBounds = false;
		int be = options.outHeight / (height / 10);
		if (be % 10 != 0) {
			be += 10;
		}
		be = be / 10;
		if (be <= 0) {
			be = 1;
		}
		options.inSampleSize = be;
		bm = BitmapFactory.decodeFile(filename, options);
		bm = Bitmap.createBitmap(bm, 0, 0, bm.getWidth(), bm.getHeight(),
				matrix, true);
		return ImageUtil.extractThumbnail(bm, width, height);
	}

	public static int rotationForImage(String filename) {
		int rotation = 0;
		try {
			ExifInterface exif = new ExifInterface(filename);
			rotation = (int) exifOrientationToDegrees(exif.getAttributeInt(
					ExifInterface.TAG_ORIENTATION,
					ExifInterface.ORIENTATION_NORMAL));
			Log.d("ORI", rotation + ",filename:" + filename);
		} catch (IOException e) {
			e.printStackTrace();
		}
		return rotation;
	}

	public static float exifOrientationToDegrees(int exifOrientation) {
		if (exifOrientation == ExifInterface.ORIENTATION_ROTATE_90) {
			return 90;
		} else if (exifOrientation == ExifInterface.ORIENTATION_ROTATE_180) {
			return 180;
		} else if (exifOrientation == ExifInterface.ORIENTATION_ROTATE_270) {
			return 270;
		}
		return 0;
	}
	public static float getFileSize(String filename){
		float fileSize = 0;
		File f = null;

		if (!TextUtils.isEmpty(filename) ) {
			f = new File(filename);
			fileSize = f.length() / 1024;
			f = null;
		}
		return fileSize;
	}
    public static String findAlbums(Context context){
		String selection = MediaStore.Images.Media.DATA + " NOT LIKE " + "'%Stream%'"
					+ " AND "+MediaStore.Images.Media.DISPLAY_NAME
					+ " NOT LIKE '%.jps%'";
		Cursor cursor = context.getContentResolver().query(
				MediaStore.Images.Media.EXTERNAL_CONTENT_URI,
				new String[]{MediaStore.Images.Media.BUCKET_DISPLAY_NAME},
				selection,
				null,
				MediaStore.Images.Media.DEFAULT_SORT_ORDER);
		StringBuilder sb = new StringBuilder();
		if(cursor!=null && cursor.getCount()>0){
			int count = cursor.getCount();
			String lastDisplayName = "";
			String thisTimeDisplayname = "";
			if(count>0){
				cursor.moveToFirst();
				for (int i = 0; i < count ; i++) {
					thisTimeDisplayname = cursor.getString(0);
					if(!thisTimeDisplayname.equals(lastDisplayName)){
						sb.append(thisTimeDisplayname).append(",");
						lastDisplayName = thisTimeDisplayname;
					}
					cursor.moveToNext();
				}
			}
		}
		if(cursor!=null){
			cursor.close();
		}
		return sb.toString();
    }

	public static void setImage(ImageView image, Bitmap bitmap) {
		if (bitmap != null) {
			Drawable[] layers = new Drawable[2];
			layers[0] = new ColorDrawable(Color.TRANSPARENT);
			layers[1] = new BitmapDrawable(bitmap);
			TransitionDrawable drawable = new TransitionDrawable(layers);
			image.setImageDrawable(drawable);
			drawable.startTransition(300);
		}
	}
}
