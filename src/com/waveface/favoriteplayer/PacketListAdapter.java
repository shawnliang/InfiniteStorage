/*
 * Copyright 2011 David Simmons
 * http://cafbit.com/entry/testing_multicast_support_on_android
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.waveface.favoriteplayer;

import java.util.LinkedList;
import java.util.ListIterator;

import android.content.Context;
import android.database.DataSetObserver;
import android.text.TextUtils;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ListAdapter;
import android.widget.TextView;

import com.waveface.favoriteplayer.mdns.DataPacket;
import com.waveface.favoriteplayer.mdns.ServerInfo;

/**
 * This class manages a list of packets to be displayed
 * in the scrollable ListView.
 * @author simmons
 */
public class PacketListAdapter implements ListAdapter {
    
    private Context context;
    private LinkedList<DataPacket> packetList;
    private LinkedList<DataSetObserver> observers = new LinkedList<DataSetObserver>();
    
    public PacketListAdapter(Context context) {
        this.context = context;
        this.packetList = new LinkedList<DataPacket>();
    }

    public PacketListAdapter(Context context, LinkedList<DataPacket> packetList) {
        this.context = context;
        this.packetList = packetList;
    }

    public boolean areAllItemsEnabled() {
        return false;
    }

    public boolean isEnabled(int position) {
        if (position >= 0 && position < packetList.size()) {
            return true;
        } else {
            return false;
        }
    }

    public int getCount() {
        return packetList.size();
    }

    public Object getItem(int position) {
        return packetList.get(position);
    }

    public long getItemId(int position) {
        // TODO: ???
        return position;
    }

    public int getItemViewType(int arg0) {
        return 0;
    }

    public View getView(int position, View convertView, ViewGroup parent) {
        View view = convertView;
        if (view == null) {
            LayoutInflater inflater =
                (LayoutInflater) context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            view = inflater.inflate(R.layout.list_item, null);
        }
        DataPacket dataPacket = packetList.get(position);
        if (dataPacket != null) {
            TextView text1 = (TextView)view.findViewById(R.id.item_header);
            TextView text2 = (TextView)view.findViewById(R.id.item_desc);
            if (text1 != null) {
                if (dataPacket.src == null || dataPacket.dst == null) {
                    text1.setText("error");
                } else {
                    String dstHost;
                    if (dataPacket.dst.isAnyLocalAddress()) {
                        dstHost = "";
                    } else {
                        dstHost = dataPacket.dst.getHostAddress();
                    }
                    text1.setText(dataPacket.src.getHostAddress()+":"+dataPacket.srcPort+" -> "+dstHost+":"+dataPacket.dstPort);
                }
            }
            if (text2 != null) {
//            	String desc = packet.description;
            	ServerInfo serverInfo = dataPacket.serverInfo;
            	StringBuilder sb = new StringBuilder();
            	if(serverInfo!=null){
	            	sb.append("serverIp:").append(dataPacket.src.getHostAddress()+"\n");            		            		
	            	sb.append("serverName:").append(TextUtils.isEmpty(serverInfo.serverName)?"\n":serverInfo.serverName+"\n");            		
	            	sb.append("serverId:").append(TextUtils.isEmpty(serverInfo.serverId)?"\n":serverInfo.serverId+"\n");
	            	sb.append("wsPort:").append(TextUtils.isEmpty(serverInfo.wsPort)?"\n":serverInfo.wsPort+"\n");
	            	sb.append("notifyPort:").append(TextUtils.isEmpty(serverInfo.notifyPort)?"\n":serverInfo.notifyPort+"\n");
	            	sb.append("restPort:").append(TextUtils.isEmpty(serverInfo.restPort)?"\n":serverInfo.restPort+"\n");
            	}
            	else{
            		sb.append("Empty!");
            	}
            	text2.setText(sb.toString());
            	
            }
        }
        return view;
    }

    public int getViewTypeCount() {
        return 1;
    }

    public boolean hasStableIds() {
        return false;
    }

    public boolean isEmpty() {
        return packetList.isEmpty();
    }

    public void registerDataSetObserver(DataSetObserver observer) {
        observers.add(observer);
    }

    public void unregisterDataSetObserver(DataSetObserver observer) {
        ListIterator<DataSetObserver> li = observers.listIterator();
        while (li.hasNext()) {
            if (li.next().equals(observer)) {
                li.remove();
            }
        }
    }

    public void addPacket(DataPacket dataPacket) {
        packetList.add(dataPacket);
        for (DataSetObserver observer : observers) {
            observer.onChanged();
        }
    }
    
    public void clear() {
        packetList.clear();
        for (DataSetObserver observer : observers) {
            observer.onChanged();
        }       
    }
    
}
