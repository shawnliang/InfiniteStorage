package com.waveface.favoriteplayer.ui;

import java.util.ArrayList;

import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.db.LabelDB;
import com.waveface.favoriteplayer.db.LabelFileView;
import com.waveface.favoriteplayer.entity.PlaybackData;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.event.PlaybackItemClickEvent;
import com.waveface.favoriteplayer.logic.ServersLogic;
import com.waveface.favoriteplayer.ui.fragment.GalleryViewFragment;
import com.waveface.favoriteplayer.ui.fragment.VideoFragment;

import de.greenrobot.event.EventBus;

import android.database.Cursor;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Environment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentTransaction;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.widget.ProgressBar;

public class VideoActivity extends FragmentActivity{
	public static final String TAG = VideoActivity.class.getSimpleName();
	private String mCurrentFragment;
	private ArrayList<PlaybackData> mDatas;
	private String mLabelTitle;
	private ProgressBar mProgress;
	
	@Override
	protected void onCreate(Bundle arg0) {
		super.onCreate(arg0);
		
		setContentView(R.layout.activity_video);
		mProgress = (ProgressBar) findViewById(R.id.progress);
		
		Bundle data = null;
		
		if(arg0 == null) {
			data = getIntent().getExtras();
		} else {
			data = arg0;
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
		
		if(GalleryViewFragment.class.getSimpleName().equals(mCurrentFragment)) {
			VideoFragment fragment = new VideoFragment();
			Bundle data = new Bundle();
			data.putParcelableArrayList(Constant.ARGUMENT1, mDatas);
			data.putInt(Constant.ARGUMENT2, event.position);
			fragment.setArguments(data);
			FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
			transaction.add(R.id.content, fragment, VideoFragment.class.getSimpleName());
			mCurrentFragment = VideoFragment.class.getSimpleName();
			transaction.setCustomAnimations(android.R.anim.fade_in, android.R.anim.fade_out);
			transaction.addToBackStack(mCurrentFragment);
			transaction.commit();
		}
		
		
	}
	
	@Override
	public void onBackPressed() {
		if(VideoFragment.class.getSimpleName().equals(mCurrentFragment)) {
			getSupportFragmentManager().popBackStack();
			mCurrentFragment = GalleryViewFragment.class.getSimpleName();
		} else {
			finish();
		}
	}
	
	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
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

			Cursor lfc = LabelDB.getLabelFileViewByLabelId(getApplicationContext(), mLabelId);
			for(int i=0; i<lfc.getCount(); ++i) {
				lfc.moveToPosition(i);
				PlaybackData pd = new PlaybackData();
				String fileName =  Environment.getExternalStorageDirectory().getAbsolutePath()
						+ Constant.VIDEO_FOLDER+ "/"  +lfc
						.getString(lfc
								.getColumnIndex(LabelFileView.COLUMN_FILE_NAME));
				pd.url = fileName;
				datas.add(pd);
			}
			lfc.close();
			
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
