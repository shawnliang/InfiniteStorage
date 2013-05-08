package com.waveface.sync.ui.fragment;

import android.app.Activity;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.RelativeLayout;
import android.widget.TextView;

import com.waveface.sync.Constant;
import com.waveface.sync.R;
import com.waveface.sync.logic.BackupLogic;
import com.waveface.sync.util.StringUtil;

public class CleanStorageFragment extends FragmentBase implements OnClickListener{
	public final String TAG = CleanStorageFragment.class.getSimpleName();
	private ViewGroup mRootView;
	private String mType ;
	private CleanStorageListener mListener;
	private RelativeLayout mRLFirstPattern ;
	private RelativeLayout mRLSecondPattern ;
	private RelativeLayout mRLThirdPattern ;

	@Override
    public void onAttach(Activity activity) {
        super.onAttach(activity);
        try {
        	mListener = (CleanStorageListener) activity;
        } catch (ClassCastException e) {
            throw new ClassCastException(activity.toString() + " must implement CleanStorageListener");
        }
    }

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle bundle) {
		
		mType = getActivity().getIntent().getStringExtra(Constant.BUNDLE_FILE_TYPE);
		mRootView = (ViewGroup) inflater.inflate(R.layout.fragment_clean_storage, null);
		
		
		TextView title = (TextView) mRootView.findViewById(R.id.tvTitle);
		String displayType = null;
		if(mType.equals(Constant.TRANSFER_TYPE_IMAGE)){
			displayType = getActivity().getString(R.string.photo);
		}
		else if(mType.equals(Constant.TRANSFER_TYPE_VIDEO)){
			displayType = getActivity().getString(R.string.video);
		}
		else if(mType.equals(Constant.TRANSFER_TYPE_AUDIO)){
			displayType = getActivity().getString(R.string.audio);
		}
		
		title.setText(getActivity().getString(R.string.clean_title, displayType));
		Button btn = (Button) mRootView.findViewById(R.id.btnCancel);
		btn.setOnClickListener(this);
		
        mRLFirstPattern = (RelativeLayout)mRootView.findViewById(R.id.llFirstPattren);
        mRLFirstPattern.setOnClickListener(this);
        TextView tv = (TextView)mRootView.findViewById(R.id.tv3MonthFreeSpace);
        String startDate = StringUtil.getLocalDate();
        String endDate =StringUtil.dateReverse(startDate, 3*31)+"T00:00:00Z";
        String space = BackupLogic.getSpaceByType(getActivity(), mType, startDate, endDate);
        tv.setText(getActivity().getString(R.string.clean_freespace,space));
        
        mRLSecondPattern = (RelativeLayout)mRootView.findViewById(R.id.llSecondPattren);
        mRLSecondPattern.setOnClickListener(this);
        tv = (TextView)mRootView.findViewById(R.id.tv6MonthFreeSpace);
        endDate =StringUtil.dateReverse(startDate, 6*31)+"T00:00:00Z";
        space = BackupLogic.getSpaceByType(getActivity(), mType, startDate, endDate);
        tv.setText(getActivity().getString(R.string.clean_freespace,space));
        
        mRLThirdPattern = (RelativeLayout)mRootView.findViewById(R.id.llThirdPattren);
        mRLThirdPattern.setOnClickListener(this);
        
		return mRootView;
	}
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
	}
	public interface CleanStorageListener {
		public void cancel();
		public void keepThreeMonths();
		public void keepSixMonths();
		public void KeepSetting();		
	}

	@Override
	public void onDestroy() {
		super.onDestroy();
	}

	@Override
	public void onClick(View v) {
		int viewId = v.getId();
		switch (viewId) {
			case R.id.btnCancel:
				mListener.cancel();
				break;
			case R.id.llFirstPattren:
				mListener.keepThreeMonths();
				break;
			case R.id.llSecondPattren:
				mListener.keepSixMonths();
				break;
			case R.id.llThirdPattren:
				mListener.KeepSetting();
				break;		
		}
	}
}