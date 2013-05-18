package com.waveface.sync.service;

import java.util.HashMap;

import android.content.Context;

import com.waveface.exception.WammerServerException;
import com.waveface.service.HttpInvoker;
import com.waveface.sync.Constant;
import com.waveface.sync.RuntimeState;
import com.waveface.sync.db.LabelDB;
import com.waveface.sync.entity.FileEntity;
import com.waveface.sync.entity.LabelEntity;

public class LabelHandle {
	
	
	public  static  void LabelChange(Context context,LabelEntity entity,String ip,String restPort){
		
		
		String restfulAPIURL ="http://"+ip+":"+restPort;
		String getFileURL = restfulAPIURL + Constant.URL_GET_FILE;
		String files=null;
		String jsonOutput=null;
		FileEntity fileEntity=null;
		
		HashMap<String, String> param = new HashMap<String, String>();
		if(entity!=null){
			for(LabelEntity.Label label: entity.labels){
				if(label.files!=null)
				if(label.files.length>0){
					 for(String f:label.files){
						  files+=f+",";
					  }
					 files=files.substring(0, files.length()-1);
					 param.clear();
					 param.put(Constant.PARAM_FILES, files.trim());
					 try {
						jsonOutput = HttpInvoker.executePost(getFileURL, param,  Constant.CLOUD_CONNECTION_TIMEOUT, Constant.CLOUD_CONNECTION_TIMEOUT);
					} catch (WammerServerException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
					 
					 //FileEntity
					 fileEntity = RuntimeState.GSON.fromJson(jsonOutput, FileEntity.class);	 
//					 Log.d(TAG, "file fileEntity ="+fileEntity);
//					 Log.d(TAG, "file jsonOutString ="+jsonOutput);
					 
				}
				// update label info
				LabelDB.updateLabelInfo(context, label, fileEntity);
			}
		}
		
		
	}

}
