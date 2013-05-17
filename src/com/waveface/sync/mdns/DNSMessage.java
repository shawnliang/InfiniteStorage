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
package com.waveface.sync.mdns;

import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.SortedMap;
import java.util.TreeMap;

import android.util.Log;

import com.waveface.sync.MDNSConstant;

/**
 * This class represents a single DNS message, and is capable
 * of parsing or constructing such a message.
 * 
 * see: http://www.ietf.org/rfc/rfc1035.txt
 * 
 * @author simmons
 */
public class DNSMessage {
    
    private static short nextMessageId = 0;

    private short messageId;
    private LinkedList<DNSQuestion> questions = new LinkedList<DNSQuestion>();
    private LinkedList<DNSAnswer> answers = new LinkedList<DNSAnswer>();

    /**
     * Construct a DNS host query
     */
    public DNSMessage(String hostname) {
        messageId = nextMessageId++;
        questions.add(new DNSQuestion(DNSQuestion.Type.ANY, hostname));
    }
    
    /**
     * Parse the supplied packet as a DNS message.
     */
    public DNSMessage(byte[] packet) {
        parse(packet, 0, packet.length);
    }
    
    /**
     * Parse the supplied packet as a DNS message.
     */
    public DNSMessage(byte[] packet, int offset, int length) {
        parse(packet, offset, length);
    }
    
    public int length() {
        int length = 12; // header length
        for (DNSQuestion q : questions) {
            length += q.length();
        }
        for (DNSAnswer a : answers) {
            length += a.length();
        }
        return length;
    }
    
    public byte[] serialize() {
        DNSBuffer buffer = new DNSBuffer(length());
        
        // header
        buffer.writeShort(messageId);
        buffer.writeShort(0); // flags
        buffer.writeShort(questions.size()); // qdcount
        buffer.writeShort(answers.size()); // ancount
        buffer.writeShort(0); // nscount
        buffer.writeShort(0); // arcount
        
        // questions
        for (DNSQuestion question : questions) {
            question.serialize(buffer);
        }
        
        // answers
        for (DNSAnswer answer : answers) {
            answer.serialize(buffer);
        }
        
        return buffer.bytes;
    }
    
    private void parse(byte[] packet, int offset, int length) {
        DNSBuffer buffer = new DNSBuffer(packet, offset, length);
        
        // header
        messageId = buffer.readShort();
        buffer.readShort(); // flags
        int qdcount = buffer.readShort();
        int ancount = buffer.readShort();
        buffer.readShort(); // nscount
        buffer.readShort(); // arcount
        
        // questions
        questions.clear();
        for (int i=0; i<qdcount; i++) {
            questions.add(new DNSQuestion(buffer));
        }
        
        // answers
        answers.clear();
        for (int i=0; i<ancount; i++) {
            answers.add(new DNSAnswer(buffer));
        }
    }
    
    public String toString() {
        StringBuilder sb = new StringBuilder();
        
        // questions
        for (DNSQuestion q : questions) {
            sb.append(q.toString()+"\n");
            Log.d("APPEND STRING", "QUESTION:"+q.toString());
        }
        
        // group answers by name
        SortedMap<String,List<DNSAnswer>> answersByName = new TreeMap<String,List<DNSAnswer>>();
        for (DNSAnswer a : answers) {
            List<DNSAnswer> list;
            if (answersByName.containsKey(a.name)) {
                list = answersByName.get(a.name);
            } else {
                list = new LinkedList<DNSAnswer>();
                answersByName.put(a.name, list);
            }
            list.add(a);
        }
        for (Map.Entry<String, List<DNSAnswer>> entry : answersByName.entrySet()) {
            sb.append(entry.getKey()+"\n");
            Log.d("APPEND STRING", "ANSWER KEY:"+entry.getKey());
            for (DNSAnswer a : entry.getValue()){
                sb.append("  "+a.type.toString()+":"+a.getRdataString()+"\n");
                Log.d("APPEND STRING", "ANSWER VALUE-->"+a.type.toString()+":"+a.getRdataString());
            }
        }
        Log.d("APPEND STRING", "END");
        return sb.toString();
    }
    public ServerInfo getServerInfo(){
    	ServerInfo serverInfo = new ServerInfo();
        // group answers by name
        SortedMap<String,List<DNSAnswer>> answersByName = new TreeMap<String,List<DNSAnswer>>();
        for (DNSAnswer a : answers) {
            List<DNSAnswer> list;
            if (answersByName.containsKey(a.name)) {
                list = answersByName.get(a.name);
            } else {
                list = new LinkedList<DNSAnswer>();
                answersByName.put(a.name, list);
            }
            list.add(a);
        }
        for (Map.Entry<String, List<DNSAnswer>> entry : answersByName.entrySet()) {
        	String entryKey = entry.getKey();
        	String dataType = null;
        	String answerValue = null;
        	String[] dataArray = null;
        	String[] keyValue = null;        	
        	int matchIndex = entryKey.indexOf(MDNSConstant.BS_ID);
        	if(matchIndex>=0){
        		if(matchIndex>0){
        			serverInfo.serverName = entryKey.substring(0, matchIndex-1);
        		}
        		else{
        			serverInfo.serverName = entryKey;
        		}
    	        Log.d("DNSMessage", "SERVER NAME:"+serverInfo.serverName);

	            for (DNSAnswer a : entry.getValue()){
	                dataType = a.type.toString();
	                answerValue = a.getRdataString();
	                if(dataType.equals("TXT")){
		                dataArray = answerValue.split(",");
	                	for(int i = 0 ; i<dataArray.length;i++){
	                		keyValue = dataArray[i].split("=");
	                		if(keyValue[0].equals("server_id")){
	                			serverInfo.serverId = keyValue[1];
	                		}
	                		else if(keyValue[0].equals("ws_port")){
	                			serverInfo.wsPort = keyValue[1];
	                		}
	                		else if(keyValue[0].equals("notify_port")){
	                			serverInfo.notifyPort = keyValue[1];
	                		}
	                		else if(keyValue[0].equals("rest_port")){
	                			serverInfo.restPort = keyValue[1];
	                		}	                		
	                	}
	                }
	                else if(dataType.equals("PTR")){
	                	matchIndex = answerValue.indexOf(MDNSConstant.BS_ID);
	            		if(matchIndex>0){
	            			serverInfo.serverName = answerValue.substring(0, matchIndex-1);
	            	        Log.d("DNSMessage", "SERVER NAME:"+serverInfo.serverName);
	            		}
	                }
	            }
        	}
        }
    	
    	return serverInfo;
    }
}
