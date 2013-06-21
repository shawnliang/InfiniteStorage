package com.waveface.favoriteplayer.ui;

import java.util.ArrayList;

import android.content.Context;
import android.content.SharedPreferences;
import android.content.SharedPreferences.OnSharedPreferenceChangeListener;
import android.os.Bundle;
import android.preference.Preference;
import android.preference.Preference.OnPreferenceClickListener;
import android.preference.PreferenceManager;
import android.text.TextUtils;
import android.widget.Toast;

import com.actionbarsherlock.app.SherlockPreferenceActivity;
import com.waveface.favoriteplayer.Constant;
import com.waveface.favoriteplayer.R;
import com.waveface.favoriteplayer.entity.ServerEntity;
import com.waveface.favoriteplayer.logic.ServersLogic;

@SuppressWarnings("deprecation")
public class SettingActivity extends SherlockPreferenceActivity implements OnSharedPreferenceChangeListener, OnPreferenceClickListener{

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		addPreferencesFromResource(R.xml.setting_activity);
		
		SharedPreferences sharedPref = getSharedPreferences(Constant.PREFS_NAME, Context.MODE_PRIVATE);
		sharedPref.registerOnSharedPreferenceChangeListener(this);
		String home_sharing = sharedPref.getString(Constant.PREF_HOME_SHARING_STATUS, "");
		
		Preference pref = findPreference(Constant.PREF_HOME_SHARING);
		if("false".equals(home_sharing)) {
			pref.setSummary(getText(R.string.disable));
		} else {
			pref.setSummary(getText(R.string.enable));
		}
		
		ArrayList<ServerEntity> servers = ServersLogic.getPairedServer(this);
		ServerEntity pairedServer = servers.get(0);
		
		pref = findPreference(Constant.PREF_CONNECT_SERVER);
		pref.setTitle(getString(R.string.connect_server, pairedServer.serverName));
		
		pref = findPreference(Constant.PREF_UNLINK_SERVER);
		pref.setOnPreferenceClickListener(this);
	}

	@Override
	public void onSharedPreferenceChanged(SharedPreferences sharedPreferences,
			String key) {
		if(key.equals(Constant.PREF_HOME_SHARING_STATUS)) {
			String home_sharing = sharedPreferences.getString(Constant.PREF_HOME_SHARING_STATUS, "");
			Preference pref = findPreference(Constant.PREF_HOME_SHARING);
			if("false".equals(home_sharing)) {
				pref.setSummary(getText(R.string.disable));
			} else {
				pref.setSummary(getText(R.string.enable));
			}
		} 
	}

	@Override
	public boolean onPreferenceClick(Preference pref) {
		if(pref.getKey().equals(Constant.PREF_UNLINK_SERVER)) {
			Toast.makeText(this, "unlink here", Toast.LENGTH_SHORT).show();
		}
		return false;
	}
}
