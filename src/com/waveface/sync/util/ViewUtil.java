package com.waveface.sync.util;

import android.app.Activity;
import android.content.Context;
import android.content.res.Configuration;
import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.DisplayMetrics;
import android.util.TypedValue;
import android.view.LayoutInflater;
import android.view.ViewGroup;
import android.view.inputmethod.InputMethodManager;

import com.waveface.sync.R;



public class ViewUtil {
	private static final String TAG = ViewUtil.class.getSimpleName();
	public static final int PORTRAIT = 1;
	public static final int LANDSCAPE = 2;
	private static final BitmapFactory.Options sBitmapOptionsCache = new BitmapFactory.Options();

    static {
        // for the cache,
        // 565 is faster to decode and display
        // and we don't want to dither here because the image will be scaled down later
        sBitmapOptionsCache.inPreferredConfig = Bitmap.Config.RGB_565;
        sBitmapOptionsCache.inDither = false;
    }


	public static int getScreenOrientation(Context context) {
		// Query what the orientation currently really is.
		if (context.getResources().getConfiguration().orientation == Configuration.ORIENTATION_PORTRAIT) {
			return PORTRAIT; // Portrait Mode

		} else if (context.getResources().getConfiguration().orientation == Configuration.ORIENTATION_LANDSCAPE) {
			return LANDSCAPE; // Landscape mode
		}
		return 0;
	}

	public static boolean qualifiedTabletLayout(Context context) {
		ViewGroup root = (ViewGroup) LayoutInflater.from(context).inflate(R.layout.layout_dummy, null);
        if (root.findViewById(R.id.seven_inch_tablet) != null || root.findViewById(R.id.ten_inch_tablet) != null)
        	return true;
        else
        	return false;
	}

	public static int convertDips(float dips, Context context) {
		int pixels = (int) TypedValue.applyDimension(
				TypedValue.COMPLEX_UNIT_DIP, dips, context.getResources()
						.getDisplayMetrics());
		return pixels;
	}

	public static float convertPixels(float px, Context context){
	    Resources resources = context.getResources();
	    DisplayMetrics metrics = resources.getDisplayMetrics();
	    float dp = px / (metrics.densityDpi / 160f);
	    return dp;

	}

	public static void hidekeyboard(Activity activity) {
		if(activity == null)
			return;
		InputMethodManager inputManager = (InputMethodManager) activity
				.getSystemService(Context.INPUT_METHOD_SERVICE);
		int SoftInputAnchor = 0;
		if(inputManager.isAcceptingText()) {
			inputManager.toggleSoftInput(SoftInputAnchor, 0);
			if(activity.getCurrentFocus() != null)
				inputManager.hideSoftInputFromWindow(activity.getCurrentFocus().getWindowToken(),
					InputMethodManager.HIDE_NOT_ALWAYS);
		}
	}
	public static int convertDipToPixels(float dips,Context context)
	{
	    return (int) (dips * context.getResources().getDisplayMetrics().density + 0.5f);
	}
}
