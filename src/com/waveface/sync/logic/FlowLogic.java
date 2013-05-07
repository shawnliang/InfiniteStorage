package com.waveface.sync.logic;

import java.net.URLEncoder;

import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.text.Html;

import com.waveface.sync.R;
import com.waveface.sync.util.DeviceUtil;

public class FlowLogic {

	public static void sendDownloadEmail(Context context) {
		String subject = context.getString(R.string.email_subject);
		String content = Html.fromHtml(context.getString(R.string.email_content)).toString();
		String mailId = DeviceUtil.getEmailAccount(context);
		String uriText = "mailto:"+mailId + 
			    "?subject=" + URLEncoder.encode(subject) + 
			    "&body=" + URLEncoder.encode(content);		
		Intent intent = new Intent(Intent.ACTION_SENDTO,Uri.parse(uriText));
		context.startActivity(intent);
	}

}
