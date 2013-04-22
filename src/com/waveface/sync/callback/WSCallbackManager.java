package com.waveface.sync.callback;

import java.util.HashMap;
import java.util.Iterator;

import android.util.Log;


public class WSCallbackManager {
	
	private HashMap<String,EventCallback> mlisteners ;
	private static WSCallbackManager mInstance ;
	
	private WSCallbackManager(){
		mlisteners = new HashMap<String,EventCallback>();
	}
	public static WSCallbackManager getInstance(){
		if( mInstance ==null){
			mInstance = new WSCallbackManager();
		}
		return mInstance;
	}
	
		
    public void register(EventCallback callback) {
    	
    	if( !mlisteners.containsKey(callback.getClass().getSimpleName())){
    		mlisteners.put(callback.getClass().getSimpleName(),callback);
    	}
    }
    public void unregister(EventCallback callback) {
    	String key = callback.getClass().getSimpleName();
    	if(mlisteners!=null && mlisteners.containsKey(key)){
    		mlisteners.remove(key);
    	}
    }
    
    public void fireEvent(String action,String content){
    	Log.d("CALLBACK", "Client count:"+getCount());
    	if(mlisteners!=null){
	    	Iterator<String> iters = mlisteners.keySet().iterator();
	    	while(iters.hasNext()){
	    		mlisteners.get(iters.next()).fired(action,content);
	    	}    	
    	}
    }
    public int getCount(){
    	if(mlisteners!=null){
    		return mlisteners.size();
    	}
    	else{
    		return 0;
    	}
    }
    public void finish(){
    	mlisteners = null;
    	mInstance = null;
    }    
}

