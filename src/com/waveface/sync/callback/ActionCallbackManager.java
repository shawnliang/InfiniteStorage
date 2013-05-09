package com.waveface.sync.callback;

import java.util.HashMap;
import java.util.Iterator;

import android.util.Log;

public class ActionCallbackManager {
	
	private static ActionCallbackManager mInstance ;
	private ActionCallbackManager(){
		
	}
	public static ActionCallbackManager getInstance(){
		if( mInstance ==null){
			mInstance = new ActionCallbackManager();
		}
		return mInstance;
	}
	
	HashMap<String,ActionCallback> mlisteners = new HashMap<String,ActionCallback>();
	
    public void register(ActionCallback callback) {
    	
    	if( !mlisteners.containsKey(callback.getClass().getSimpleName())){
    		mlisteners.put(callback.getClass().getSimpleName(),callback);
    	}
    }
    public void unregister(ActionCallback callback) {
    	String key = callback.getClass().getSimpleName();
    	if(mlisteners!=null && mlisteners.containsKey(key)){
    		mlisteners.remove(key);
    	}
    }
    
    public void fireEvent(String action,int value){
    	Log.d("CALLBACK", "Client count:"+getCount());
    	if(mlisteners!=null){
	    	Iterator<String> iters = mlisteners.keySet().iterator();
	    	while(iters.hasNext()){
	    		mlisteners.get(iters.next()).fired(action,value);
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
}
