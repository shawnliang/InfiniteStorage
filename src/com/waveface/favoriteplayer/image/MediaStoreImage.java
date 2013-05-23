package com.waveface.favoriteplayer.image;


import java.io.FileNotFoundException;
import java.io.IOException;

import android.content.ContentResolver;
import android.content.ContentUris;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Matrix;
import android.net.Uri;
import android.os.ParcelFileDescriptor;
import android.provider.MediaStore;
import android.util.Log;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.util.FileUtil;
import com.waveface.favoriteplayer.util.ImageUtil;

public class MediaStoreImage  {
	private static final String TAG = MediaStoreImage.class.getSimpleName();
	private final Context mContext;
	private final int IMAGE_MAX_WIDTH;
	private final int IMAGE_MAX_HEIGHT;

	private static final BitmapFactory.Options sBitmapOptionsCache = new BitmapFactory.Options();
	private static final Uri sArtworkUri = Uri.parse("content://media/external/audio/albumart");

    static {
        // for the cache,
        // 565 is faster to decode and display
        // and we don't want to dither here because the image will be scaled down later
        sBitmapOptionsCache.inPreferredConfig = Bitmap.Config.RGB_565;
        sBitmapOptionsCache.inDither = false;
    }

	public MediaStoreImage(Context context, int width, int height) {
		mContext = context;
		IMAGE_MAX_WIDTH = width;
		IMAGE_MAX_HEIGHT = height;
	}


	public Bitmap getBitmap(long mediaId,int type,String filename) {
	    Bitmap b = null;
	    switch(type){
	    case Constant.TYPE_IMAGE:
		    	b = MediaStore.Images.Thumbnails.getThumbnail(
		    			mContext.getContentResolver(), mediaId,
						MediaStore.Images.Thumbnails.MINI_KIND, sBitmapOptionsCache);
		    	break;
	    case Constant.TYPE_VIDEO :
		    	b = MediaStore.Video.Thumbnails.getThumbnail(
		    			mContext.getContentResolver(), mediaId,
		    			MediaStore.Images.Thumbnails.MINI_KIND, sBitmapOptionsCache);
		    	break;
	    case Constant.TYPE_AUDIO:
			    b = getArtworkQuick(mContext, mediaId, IMAGE_MAX_WIDTH, IMAGE_MAX_HEIGHT);
			    break;
	    }
	    if(b != null) {
	    	// we don't need the image to big, but hence android Thumbnails.MICRO_KIND
	    	// will have problem to retrieve right image, we need to re-scale bitmap
	    	if(FileUtil.isFileExisted(filename)){
		    	b = ImageUtil.extractThumbnail(b, IMAGE_MAX_WIDTH, IMAGE_MAX_HEIGHT);
	
		    	float rotation = ImageUtil.rotationForImage(filename);
				if (rotation != 0f) {
					Matrix matrix = new Matrix();
					matrix.preRotate(rotation);
					try{
						b = Bitmap.createBitmap(b, 0, 0, IMAGE_MAX_WIDTH,IMAGE_MAX_HEIGHT,matrix, true);
					}
					catch(Exception ex){
						ex.printStackTrace();
					}
				}
	    	}
	    }
	    return b;
	}

	private static Bitmap getArtworkQuick(Context context, long mediaId,
			int w, int h) {
		// NOTE: There is in fact a 1 pixel border on the right side in the
		// ImageView used to display this drawable.
		// Take it into account now, so we don't have to scale later.
		w -= 1;
		ContentResolver res = context.getContentResolver();
		Uri uri = Uri.parse("content://media/external/audio/media/"
				+ mediaId + "/albumart");
		if (uri != null) {
			ParcelFileDescriptor pfd = null;
			try {
				pfd = res.openFileDescriptor(uri, "r");
				if(pfd == null) {
					Log.e(TAG, "ParcelFileDescriptor is null");
					uri = ContentUris.withAppendedId(sArtworkUri, mediaId);
					pfd = res.openFileDescriptor(uri, "r");
				}
				if(pfd == null) {
					Log.e(TAG, "ParcelFileDescriptor still null");
					return null;
				}
				int sampleSize = 1;

				// Compute the closest power-of-two scale factor
				// and pass that to sBitmapOptionsCache.inSampleSize, which will
				// result in faster decoding and better quality
				sBitmapOptionsCache.inJustDecodeBounds = true;
				BitmapFactory.decodeFileDescriptor(pfd.getFileDescriptor(),
						null, sBitmapOptionsCache);
				int nextWidth = sBitmapOptionsCache.outWidth >> 1;
				int nextHeight = sBitmapOptionsCache.outHeight >> 1;
				while (nextWidth > w && nextHeight > h) {
					sampleSize <<= 1;
					nextWidth >>= 1;
					nextHeight >>= 1;
				}

				sBitmapOptionsCache.inSampleSize = sampleSize;
				sBitmapOptionsCache.inJustDecodeBounds = false;
				Bitmap b = BitmapFactory.decodeFileDescriptor(
						pfd.getFileDescriptor(), null, sBitmapOptionsCache);

				if (b != null) {
					// finally rescale to exactly the size we need
					if (sBitmapOptionsCache.outWidth != w
							|| sBitmapOptionsCache.outHeight != h) {
						Bitmap tmp = Bitmap.createScaledBitmap(b, w, h, true);
						// Bitmap.createScaledBitmap() can return the same
						// bitmap
						if (tmp != b)
							b.recycle();
						b = tmp;
					}
				}

				return b;
			} catch (FileNotFoundException e) {
				Log.e(TAG,  "getArtworkQuick : FileNotFoundException");
			} finally {
				try {
					if (pfd != null)
						pfd.close();
				} catch (IOException e) {
				}
			}
		}
		return null;
	}
}
