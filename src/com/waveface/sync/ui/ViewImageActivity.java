package com.waveface.sync.ui;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.media.MediaPlayer;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
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
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.db.ImportFilesTable;
import com.waveface.sync.image.MediaStoreImage;
import com.waveface.sync.util.StringUtil;

public class ViewImageActivity extends Activity {

	private ListView listview;
	private DisplayMetrics metrics;
	private int mode = 2;
    private MediaStoreImage mMediaImage;
    private MainAdapter mAdapter ; 
    private ImageEntity mEntity;
    private MediaPlayer mMediaPlayer;
    private int mDisplayType = 0;
    
    
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
		if(mType.equals(Constant.TRANSFER_TYPE_IMAGE)){
			mDisplayType = Constant.TYPE_IMAGE;
		}
		else if(mType.equals(Constant.TRANSFER_TYPE_VIDEO)){
			mDisplayType = Constant.TYPE_VIDEO;
		}
		else if(mType.equals(Constant.TRANSFER_TYPE_AUDIO)){
			mDisplayType = Constant.TYPE_AUDIO;
		}
	
		Cursor cursor = getContentResolver()
				.query(ImportFilesTable.CONTENT_URI, 
						new String[]{
						ImportFilesTable.COLUMN_IMAGE_ID,
						ImportFilesTable.COLUMN_FILENAME
				}, ImportFilesTable.COLUMN_FILETYPE+"=?",
				new String[]{String.valueOf(mDisplayType)}, 
				ImportFilesTable.COLUMN_DATE+" DESC");
		ArrayList<ImageEntity> entities = new ArrayList<ImageEntity>();
		if(cursor!=null && cursor.getCount()>0){
			cursor.moveToFirst();
			for(int i = 0 ; i<cursor.getCount(); i++ ){
				ImageEntity entity = new ImageEntity();
				entity.mediaId = cursor.getLong(0);
				entity.filename = cursor.getString(1);
				entities.add(entity);
				cursor.moveToNext();
			}
			
		}
		cursor.close();
		mAdapter = new MainAdapter(this, entities, metrics,mDisplayType);
		listview.setAdapter(mAdapter);
		listview.setOnItemClickListener(new AdapterView.OnItemClickListener() {

		      @Override
		      public void onItemClick(AdapterView<?> parent, final View view,
		          int position, long id) {
		        mEntity = mAdapter.getItem(position);
		        if(mDisplayType== Constant.TYPE_AUDIO){
			        if(mMediaPlayer == null){
			        	mMediaPlayer = new MediaPlayer();
			        }
		        	if(mMediaPlayer.isPlaying()){
			        	mMediaPlayer.stop();
			        }
			        try {
						mMediaPlayer.setDataSource(mEntity.filename);
						mMediaPlayer.prepare();
						mMediaPlayer.start();
					} catch (IllegalArgumentException e) {
						e.printStackTrace();
					} catch (SecurityException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					} catch (IllegalStateException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					} catch (IOException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
		        }
		        else{		        	
		            Intent intent = new Intent();
		            intent.setAction(android.content.Intent.ACTION_VIEW);
		            File file = new File(mEntity.filename);
		            if(mDisplayType== Constant.TYPE_VIDEO){
		            	intent.setDataAndType(Uri.fromFile(file), "video/*");
		            }
		            else{
		            	intent.setDataAndType(Uri.fromFile(file), "image/*");		            	
		            }
		            startActivity(intent); 
		        }
		      }
		});		
		setContentView(listview);
	}
	public void openInGallery(String imageId) {
		  Uri uri = MediaStore.Images.Media.EXTERNAL_CONTENT_URI.buildUpon().appendPath(imageId).build();
		  Intent intent = new Intent(Intent.ACTION_VIEW, uri);
		  startActivity(intent);
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
			public ImageView playMark;			
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
						R.layout.item_viwe_image, null);
				convertView.setBackgroundColor(0xFF202020);

				holder = new Holder();
				holder.imageview = (ImageView)convertView.findViewById(R.id.ivImage);
				holder.playMark = (ImageView)convertView.findViewById(R.id.ivPlayVideo);				
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
			else{
				holder.imageview.setImageResource(R.drawable.ic_audio);
			}
			if(this.displayType == Constant.TYPE_IMAGE){
				holder.playMark.setVisibility(View.GONE);
			}
			else{
				holder.playMark.setVisibility(View.VISIBLE);
			}
			holder.textview.setText(StringUtil.getFilename(entity.filename));

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
		public ImageEntity getItem(int position) {
			return entities.get(position);
		}

		@Override
		public long getItemId(int position) {
			return 0;
		}
	}
}