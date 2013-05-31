package com.waveface.favoriteplayer.ui;

import java.util.ArrayList;

import android.database.Cursor;
import android.os.AsyncTask;
import android.os.Bundle;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.widget.ProgressBar;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileTable;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.PlaybackItemClickEvent;
import com.waveface.favoriteplayer.event.PlaybackCancelEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.fragment.FullScreenSlideshowFragment;
import com.waveface.favoriteplayer.ui.fragment.GalleryViewFragment;
import com.waveface.favoriteplayer.ui.fragment.PlaybackFragment;

import de.greenrobot.event.EventBus;

public class PlaybackActivity extends FragmentActivity {
	public static final String TAG = PlaybackActivity.class.getSimpleName();
	private String mCurrentFragment;
	private ArrayList<PlaybackData> mDatas;
	private String mLabelTitle;
	private ProgressBar mProgress;
	
	@Override
	protected void onCreate(Bundle savedInstance) {
		super.onCreate(savedInstance);

		overridePendingTransition(R.anim.fade_in, R.anim.fade_out);
		
		setContentView(R.layout.activity_full_screen_slideshow);
		
		mProgress = (ProgressBar) findViewById(R.id.progress);
		
		Bundle data = null;
		if(savedInstance == null) {
			data = getIntent().getExtras();
		} else {
			data = savedInstance;
		}
		
		String labelId = data.getString(Constant.ARGUMENT1);
		mLabelTitle = data.getString(Constant.ARGUMENT3);
		
		new LoadPlaybackData(labelId).execute(null, null, null);
		

	}
	
	@Override
	protected void onResume() {
		super.onResume();
		EventBus.getDefault().register(this);
	}
	
	@Override
	protected void onPause() {
		super.onPause();
		EventBus.getDefault().unregister(this);
	}
	
	public void onEvent(PlaybackItemClickEvent event) {
		Log.d(TAG, "PhotoItemClickEvent currentFragment=" + mCurrentFragment);
		
		if(PlaybackFragment.class.getSimpleName().equals(mCurrentFragment)) {
			Bundle data = new Bundle();
			data.putParcelableArrayList(Constant.ARGUMENT1, mDatas);
			data.putInt(Constant.ARGUMENT2, event.position);
	
			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
			FullScreenSlideshowFragment fragment = new FullScreenSlideshowFragment();
			
			fragment.setArguments(data);
			mCurrentFragment = FullScreenSlideshowFragment.class.getSimpleName();
			transaction.addToBackStack(null);
			transaction.replace(R.id.content, fragment, mCurrentFragment).commit();
		} else {
			Bundle data = new Bundle();
			data.putParcelableArrayList(Constant.ARGUMENT1, mDatas);
			data.putInt(Constant.ARGUMENT2, event.position);
			data.putString(Constant.ARGUMENT3, mLabelTitle);
	
			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
			PlaybackFragment fragment = new PlaybackFragment();
			
			fragment.setArguments(data);
			mCurrentFragment = PlaybackFragment.class.getSimpleName();
			transaction.addToBackStack(null);
			transaction.replace(R.id.content, fragment, mCurrentFragment).commit();		
		}
	}
	
	public void onEvent(PlaybackCancelEvent event) {
		getSupportFragmentManager().popBackStack();
		mCurrentFragment = PlaybackFragment.class.getSimpleName();

		PlaybackFragment fragment = (PlaybackFragment) getSupportFragmentManager().findFragmentByTag(mCurrentFragment);
		fragment.pause(event.position);
	}
	
	@Override
	public void onBackPressed() {
		Log.d(TAG, "onBackPressed:" + mCurrentFragment);
		if(GalleryViewFragment.class.getSimpleName().equals(mCurrentFragment)) {
			GalleryViewFragment fragment = (GalleryViewFragment) getSupportFragmentManager().findFragmentByTag(mCurrentFragment);
			fragment.fadeOut();
			finish();
			overridePendingTransition(android.R.anim.fade_in, android.R.anim.fade_out);
		} else {
			getSupportFragmentManager().popBackStack();
			if(FullScreenSlideshowFragment.class.getSimpleName().equals(mCurrentFragment)) {
				mCurrentFragment = PlaybackFragment.class.getSimpleName();
			} else {
				mCurrentFragment = GalleryViewFragment.class.getSimpleName();
			}
		}
	}
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		Log.d(TAG, "onKeyDown:" + keyCode);
		if(PlaybackFragment.class.getSimpleName().equals(mCurrentFragment) && keyCode != KeyEvent.KEYCODE_BACK) {
			PlaybackFragment fragment = (PlaybackFragment) getSupportFragmentManager().findFragmentByTag(mCurrentFragment);
			fragment.onKeyEvent(keyCode, event);
			return true;
		} else if(FullScreenSlideshowFragment.class.getSimpleName().equals(mCurrentFragment)){
			FullScreenSlideshowFragment fragment = (FullScreenSlideshowFragment) getSupportFragmentManager().findFragmentByTag(mCurrentFragment);
			fragment.onKeyEvent(keyCode, event);
			return true;
		} else {
			Log.d(TAG, "mCurrentFragment=" + mCurrentFragment);
		}
		return super.onKeyDown(keyCode, event);
	}
	
	class LoadPlaybackData extends AsyncTask<Void, Void, ArrayList<PlaybackData>> {
		public String mLabelId;
		
		public LoadPlaybackData(String labelId) {
			mLabelId = labelId;
		}

		@Override
		protected ArrayList<PlaybackData> doInBackground(Void... params) {
			ArrayList<ServerEntity> servers = ServersLogic.getPairedServer(getApplicationContext());
			ServerEntity pairedServer = servers.get(0);
			if(TextUtils.isEmpty(pairedServer.restPort)){
				pairedServer.restPort ="14005";
			}
			String serverUrl ="http://"+pairedServer.ip+":"+pairedServer.restPort;
			Log.d(TAG, "mServerUrl:" +serverUrl);
			
			ArrayList<PlaybackData> datas = new ArrayList<PlaybackData>();
			Cursor c = LabelDB.getLabelFilesByLabelId(getApplicationContext(), mLabelId);
			for(int i=0; i<c.getCount(); ++i) {
				c.moveToPosition(i);
				PlaybackData pd = new PlaybackData();
				pd.url = serverUrl + Constant.URL_IMAGE + "/" +
						c.getString(c.getColumnIndex(LabelFileTable.COLUMN_FILE_ID)) +
						Constant.URL_IMAGE_LARGE;
				datas.add(pd);
			}
			c.close();
			return datas;
		}
		
		@Override
		protected void onPostExecute(ArrayList<PlaybackData> result) {
			mDatas = result;
			Bundle data = new Bundle();
			data.putParcelableArrayList(Constant.ARGUMENT1, result);
			data.putString(Constant.ARGUMENT3, mLabelTitle);

			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
			mCurrentFragment = GalleryViewFragment.class.getSimpleName();
			GalleryViewFragment playback = new GalleryViewFragment();
			playback.setArguments(data);
			transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out);
			
			transaction.add(R.id.content, playback, mCurrentFragment);
			transaction.commit();
			
			mProgress.setVisibility(View.INVISIBLE);
		}
		
	}
}
