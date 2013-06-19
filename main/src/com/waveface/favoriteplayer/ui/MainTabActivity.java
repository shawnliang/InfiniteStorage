package com.waveface.favoriteplayer.ui;

import android.app.Activity;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentTransaction;
import android.widget.Toast;

import com.actionbarsherlock.app.ActionBar;
import com.actionbarsherlock.app.ActionBar.Tab;
import com.actionbarsherlock.app.SherlockFragmentActivity;
import com.actionbarsherlock.view.Menu;
import com.actionbarsherlock.view.MenuItem;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.ui.fragment.OverviewFragment;


public class MainTabActivity extends SherlockFragmentActivity{
	public static final String TAG = MainTabActivity.class.getSimpleName();
	
	private String mCurrentFragment;
	
	@Override
	public void onBackPressed() {
		if(mCurrentFragment == null) {
			super.onBackPressed();
		} else {
			getSupportFragmentManager().popBackStack();
			mCurrentFragment = null;
		}
	}
	
	@Override
	protected void onCreate(Bundle arg0) {
		super.onCreate(arg0);
		
        setContentView(R.layout.activity_maintab);
        
        Bundle data = new Bundle();
        
        getSupportActionBar().setNavigationMode(ActionBar.NAVIGATION_MODE_TABS);
        ActionBar.Tab tab = getSupportActionBar().newTab();
        
        data = new Bundle();
        data.putInt(Constant.ARGUMENT1, OverviewFragment.OVERVIEW_VIEW_TYPE_FAVORITE);
        tab.setText(R.string.favorite);
        tab.setTabListener(new TabListener<OverviewFragment>(this, "favorite", OverviewFragment.class, data));
        getSupportActionBar().addTab(tab);
        
        data = new Bundle();
        data.putInt(Constant.ARGUMENT1, OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_PHOTO);
        tab = getSupportActionBar().newTab();
        tab.setText(R.string.recent_photos);
        tab.setTabListener(new TabListener<OverviewFragment>(this, "recentphotos", OverviewFragment.class, data));
        getSupportActionBar().addTab(tab);
        
        data = new Bundle();
        data.putInt(Constant.ARGUMENT1, OverviewFragment.OVERVIEW_VIEW_TYPE_RECENT_VIDEO);
        tab = getSupportActionBar().newTab();
        tab.setText(R.string.recent_videos);
        tab.setTabListener(new TabListener<OverviewFragment>(this, "recentvideos", OverviewFragment.class, data));
        getSupportActionBar().addTab(tab);
	}
	
	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		menu.add(0, 0, 0,R.string.setting_title);
	    return super.onCreateOptionsMenu(menu);
	}
	
	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		switch(item.getItemId()) {
		case 0:
			Toast.makeText(this, "go setting", Toast.LENGTH_SHORT).show();
			break;
		}
		return super.onOptionsItemSelected(item);
	}
	
	public static class TabListener<T extends Fragment> implements ActionBar.TabListener {
	    private Fragment mFragment;
	    private final Activity mActivity;
	    private final String mTag;
	    private final Class<T> mClass;
	    private Bundle mData;

	    /** Constructor used each time a new tab is created.
	      * @param activity  The host Activity, used to instantiate the fragment
	      * @param tag  The identifier tag for the fragment
	      * @param clz  The fragment's Class, used to instantiate the fragment
	      */
	    public TabListener(Activity activity, String tag, Class<T> clz, Bundle data) {
	        mActivity = activity;
	        mTag = tag;
	        mClass = clz;
	        mData = data;
	    }

	    /* The following are each of the ActionBar.TabListener callbacks */

	    public void onTabSelected(Tab tab, FragmentTransaction ft) {
	        // Check if the fragment is already initialized
	        if (mFragment == null) {
	            // If not, instantiate and add it to the activity
	            mFragment = Fragment.instantiate(mActivity, mClass.getName());
	            mFragment.setArguments(mData);
	            ft.add(android.R.id.content, mFragment, mTag);
	        } else {
	            // If it exists, simply attach it in order to show it
	            ft.attach(mFragment);
	        }
	    }

	    public void onTabUnselected(Tab tab, FragmentTransaction ft) {
	        if (mFragment != null) {
	            // Detach the fragment, because another one is being attached
	            ft.detach(mFragment);
	        }
	    }

	    public void onTabReselected(Tab tab, FragmentTransaction ft) {
	        // User selected the already selected tab. Usually do nothing.
	    }
	}
	
}
