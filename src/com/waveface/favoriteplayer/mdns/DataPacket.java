package com.waveface.favoriteplayer.mdns;

import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;


public class DataPacket {
    public InetAddress src;
    public int srcPort;
    public InetAddress dst;
    public int dstPort;
    public String description;
    public ServerInfo serverInfo;
    public String serverIp;
	public String serverId;
	public String serverName;
	public String wsPort;
	public String notifyPost;
	public String restPort;	
	public boolean isConnected;
    
    public DataPacket() {}
    public DataPacket(DatagramPacket dp, DatagramSocket socket) {
        src = dp.getAddress();
        srcPort = dp.getPort();
        dst = socket.getLocalAddress();
        dstPort = socket.getLocalPort();
        isConnected = socket.isConnected();

        if(src!=null){
        	serverIp = src.getHostAddress();
        }
    }
}
