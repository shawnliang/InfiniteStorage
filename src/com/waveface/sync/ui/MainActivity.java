package com.waveface.sync.ui;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.logic.FileImport;
import com.waveface.sync.util.DeviceUtil;
import com.waveface.sync.util.StringUtil;
import com.waveface.sync.util.SystemUiHider;

/**
 * An example full-screen activity that shows and hides the system UI (i.e.
 * status bar and navigation/system bar) with user interaction.
 * 
 * @see SystemUiHider
 */
public class MainActivity extends Activity {

	private TextView mDevice;
	private TextView mNowPeriod;
	
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.sync_main);
		mDevice = (TextView) this.findViewById(R.id.textDevice);
		String value = DeviceUtil.getEmailAccount(this);
		value = value.split("@")[0];
		String displayText = value +"-"+DeviceUtil.getDeviceName();
		mDevice.setText(displayText);
		mNowPeriod = (TextView) this.findViewById(R.id.textPeriod);
		String[] periods = FileImport.getFilePeriods(this);
		displayText = "From "+periods[0]+" to "+periods[1];
		mNowPeriod.setText(displayText);
		
		long[] datas = FileImport.getFileInfo(this, Constant.TYPE_IMAGE);
		TextView tv = (TextView) this.findViewById(R.id.textPhotoCount);
		displayText = "Total "+datas[0]+" photos.";
		tv.setText(displayText);
		tv = (TextView) this.findViewById(R.id.textPhotoSize);
		displayText = "Size:"+StringUtil.byteCountToDisplaySize(datas[1]);
		tv.setText(displayText);
	
		datas = FileImport.getFileInfo(this, Constant.TYPE_VIDEO);
		tv = (TextView) this.findViewById(R.id.textVideoCount);
		displayText = "Total "+datas[0]+" videos.";
		tv.setText(displayText);
		tv = (TextView) this.findViewById(R.id.textVideoSize);
		displayText = "Size:"+StringUtil.byteCountToDisplaySize(datas[1]);
		tv.setText(displayText);
	
	}

    @Override
    protected void onPause() {
        super.onPause();
    }
    
    @Override
    protected void onResume() {
        super.onResume();
    }
    
    @Override
    protected void onDestroy() {
        super.onDestroy();
    }

}
