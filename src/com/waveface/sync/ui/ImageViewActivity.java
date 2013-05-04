package com.waveface.sync.ui;

import java.io.File;
import java.util.ArrayList;

import android.app.Activity;
import android.content.Context;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.os.Bundle;
import android.support.v4.widget.CursorAdapter;
import android.text.TextUtils;
import android.util.DisplayMetrics;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.Animation;
import android.view.animation.ScaleAnimation;
import android.view.animation.TranslateAnimation;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.util.StringUtil;

public class ImageViewActivity extends Activity {

	private ListView listview;
	private DisplayMetrics metrics;
	private int mode = 2;
    private MediaStoreImage mMediaImage;
    
	public static boolean isFileExisted(String filePath) {
		if (TextUtils.isEmpty(filePath))
			return false;
		return new File(filePath).exists();
	}

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		mMediaImage = new MediaStoreImage(this,120,120);
		// FROM CAMERA
		metrics = new DisplayMetrics();
		getWindowManager().getDefaultDisplay().getMetrics(metrics);

		listview = new ListView(this);
		listview.setFadingEdgeLength(0);

		String mType = getIntent().getStringExtra(Constant.BUNDLE_FILE_TYPE);
		int displayType = 0;
		if(mType.equals(Constant.TRANSFER_TYPE_IMAGE)){
			displayType = Constant.TYPE_IMAGE;
		}
		else if(mType.equals(Constant.TRANSFER_TYPE_VIDEO)){
			displayType = Constant.TYPE_VIDEO;
		}
	
		Cursor cursor = getContentResolver()
				.query(ImportFilesTable.CONTENT_URI, 
						new String[]{
						ImportFilesTable.COLUMN_IMAGE_ID,
						ImportFilesTable.COLUMN_FILENAME
				}, ImportFilesTable.COLUMN_FILETYPE+"=?",
				new String[]{String.valueOf(displayType)}, 
				ImportFilesTable.COLUMN_DATE+" DESC");
		ArrayList<ImageEntity> entities = new ArrayList<ImageEntity>();
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			for(int i = 0 ; i<cursor.getCount(); i++ ){
				ImageEntity entity = new ImageEntity();
				entity.mediaId = cursor.getLong(0);
				entity.filename = StringUtil.getFilename(cursor.getString(1));
				entities.add(entity);
				cursor.moveToNext();
			}
			
		}
		cursor.close();
		MainAdapter mAdapter = new MainAdapter(this, entities, metrics,displayType);
		listview.setAdapter(mAdapter);
		
//		ImagesListAdapter mAdapter = new ImagesListAdapter(this,cursor);
//		listview.setAdapter(mAdapter);
		setContentView(listview);
	}

	public boolean onCreateOptionsMenu(Menu menu) {
		boolean result = super.onCreateOptionsMenu(menu);
		menu.add(Menu.NONE, 1, 0, "1");
		menu.add(Menu.NONE, 2, 0, "2");
		menu.add(Menu.NONE, 3, 0, "3");
		menu.add(Menu.NONE, 4, 0, "4");
		return result;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		mode = item.getItemId();
		return super.onOptionsItemSelected(item);
	}
	class ImageEntity {
		public long mediaId;
		public String filename;
	}
	public class MainAdapter extends BaseAdapter  {
		private Context context;
		private LayoutInflater mInflater;
		private ArrayList<ImageEntity> entities;
		private DisplayMetrics metrics_;
		private int displayType;

		private class Holder {
			public ImageView imageview;
			public TextView textview;
		}

		public MainAdapter(Context context, ArrayList<ImageEntity> strings,
				DisplayMetrics metrics,int type) {
			this.context = context;
			this.mInflater = (LayoutInflater) this.context
					.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			this.entities = strings;
			this.metrics_ = metrics;
			this.displayType = type;
		}

		@Override
		public View getView(final int position, View convertView,
				ViewGroup parent) {
			final ImageEntity entity = this.entities.get(position);
			final Holder holder;

			if (convertView == null) {
				convertView = mInflater.inflate(
						R.layout.image_list_item, null);
				convertView.setBackgroundColor(0xFF202020);

				holder = new Holder();
				holder.imageview = (ImageView)convertView.findViewById(R.id.ivImage);
				holder.textview = (TextView) convertView.findViewById(R.id.tvFilename);
				
				holder.textview.setTextColor(0xFFFFFFFF);

				convertView.setTag(holder);
			} else {
				holder = (Holder) convertView.getTag();
			}

			Bitmap b = mMediaImage.getBitmap(entity.mediaId, this.displayType);
			if(b!=null){
				holder.imageview.setImageBitmap(b);
			}
			holder.textview.setText(entity.filename);

			Animation animation = null;

			switch (mode) {
			case 1:
				animation = new TranslateAnimation(metrics_.widthPixels / 3, 0,
						metrics_.widthPixels / 3, 0);
				break;

			case 2:
				animation = new TranslateAnimation(0, 0, metrics_.heightPixels,
						0);
				break;

			case 3:
				animation = new ScaleAnimation((float) 1.0, (float) 1.0,
						(float) 0, (float) 1.0);
				break;

			case 4:
				// non animation
				animation = new Animation() {
				};
				break;
			}

			animation.setDuration(750);
			convertView.startAnimation(animation);
			animation = null;

			return convertView;
		}

		@Override
		public int getCount() {
			return entities.size();
		}

		@Override
		public Object getItem(int position) {
			return entities.get(position);
		}

		@Override
		public long getItemId(int position) {
			return 0;
		}
	}
	class ImagesListAdapter extends CursorAdapter
	{
		LayoutInflater inflater;
	
		public ImagesListAdapter(Context context, Cursor c) {
			super(context, c);
			inflater = LayoutInflater.from(context);
		}

		@Override
		public void bindView(View view, Context context, Cursor cursor) {
			ImageView iv = (ImageView)view.findViewById(R.id.ivImage);
			TextView tv2 = (TextView)view.findViewById(R.id.tvFilename);
	
			Bitmap b = mMediaImage.getBitmap(cursor.getLong(0), Constant.TYPE_IMAGE);
			if(b!=null){
				iv.setImageBitmap(b);
			}
			tv2.setText(cursor.getString(1));
		}
	
		@Override
		public View newView(Context context, Cursor cursor, ViewGroup parent) {
			return inflater.inflate(R.layout.image_list_item, parent, false);
		}
	}
}