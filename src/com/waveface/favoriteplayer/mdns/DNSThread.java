package com.waveface.favoriteplayer.mdns;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.net.NetworkInterface;
import java.util.Queue;
import java.util.Set;
import java.util.concurrent.ConcurrentLinkedQueue;

import android.content.Context;
import android.net.wifi.WifiManager.MulticastLock;
import android.text.TextUtils;
import android.util.Log;

import com.waveface.favoriteplayer.MDNSConstant;
import com.waveface.favoriteplayer.MulticastTestActivity;
import com.waveface.favoriteplayer.mdns.util.NetUtility;

public class DNSThread extends Thread {

    public static final String TAG = MulticastTestActivity.TAG;
    
    private static final byte[] MDNS_ADDR =
        new byte[] {(byte) 224,(byte) 0,(byte) 0,(byte) 251};
    private static final int MDNS_PORT = 5353;

    private static final int BUFFER_SIZE = 4096;

    private NetworkInterface networkInterface;
    private InetAddress groupAddress;
    private MulticastSocket multicastSocket;
    private NetUtility net;
    private Context mContext;
    /**
     * Construct the network thread.
     * @param activity
     */
    public DNSThread(Context context) {
        super("DNSThread");
        mContext = context;
        net = new NetUtility(context);
    }
    
    /**
     * Open a multicast socket on the mDNS address and port.
     * @throws IOException
     */
    private void openSocket() throws IOException {
        multicastSocket = new MulticastSocket(MDNS_PORT);
        multicastSocket.setTimeToLive(255);
        multicastSocket.setReuseAddress(true);
        multicastSocket.setNetworkInterface(networkInterface);
        multicastSocket.joinGroup(groupAddress);
    }

    @Override
    public void run() {
        Log.v(TAG, "starting network thread");

        Set<InetAddress> localAddresses = NetUtility.getLocalAddresses();
        MulticastLock multicastLock = null;
        
        // initialize the network
        try {
            networkInterface = net.getFirstWifiOrEthernetInterface();
            if (networkInterface == null) {
                throw new IOException("Your WiFi is not enabled.");
            }
            groupAddress = InetAddress.getByAddress(MDNS_ADDR); 

            multicastLock = net.getWifiManager().createMulticastLock(MDNSConstant.BONJOUR_NAME);
            multicastLock.acquire();
            //Log.v(TAG, "acquired multicast lock: "+multicastLock);

            openSocket();
        } catch (IOException e1) {
        	Log.e(TAG, "cannot initialize network.");
            return;
        }

        // set up the buffer for incoming packets
        byte[] responseBuffer = new byte[BUFFER_SIZE];
        DatagramPacket response = new DatagramPacket(responseBuffer, BUFFER_SIZE);
        submitQuery(MDNSConstant.BONJOUR_SERVICE_ID);
        // loop!
        while (true) {
            // zero the incoming buffer for good measure.
            java.util.Arrays.fill(responseBuffer, (byte) 0); // clear buffer
            
            // receive a packet (or process an incoming command)
            try {
                multicastSocket.receive(response);
            } catch (IOException e) {
                // check for commands to be run
                Command cmd = commandQueue.poll();
                if (cmd == null) {
                    return;
                }

                // reopen the socket
                try {
                    openSocket();
                } catch (IOException e1) {
                    Log.e(TAG, "socket reopen: ");
                    e1.printStackTrace();
                    return;
                }

                // process commands
                if (cmd instanceof QueryCommand) {
                    try {
                        query(((QueryCommand)cmd).host);
                    } catch (IOException e1) {
                        e1.printStackTrace();
                    }
                } else if (cmd instanceof QuitCommand) {
                    break;
                }
                
                continue;
            }
            
            // ignore our own packet transmissions.
            if (localAddresses.contains(response.getAddress())) {
                continue;
            }
            
            // parse the DNS packet
            DNSMessage message;
            try {
                message = new DNSMessage(response.getData(), response.getOffset(), response.getLength());
            } catch (Exception e) {
            	Log.e(TAG,e.getMessage());
            	e.printStackTrace();
                continue;
            }

            DataPacket dataPacket = new DataPacket(response, multicastSocket);
            if (dataPacket.src != null && dataPacket.dst != null) {	            	 
	            dataPacket.serverInfo = message.getServerInfo();
	            if(!TextUtils.isEmpty(dataPacket.serverInfo.serverName)){
	            	MatchedServer.addServer(mContext,dataPacket);
	            }
            }
        }
        
        // release the multicast lock
        multicastLock.release();
        multicastLock = null;
        Log.v(TAG, "stopping network thread");
    }
    
    /**
     * Transmit an mDNS query on the local network.
     * @param host
     * @throws IOException
     */
    private void query(String host) throws IOException {
        byte[] requestData = (new DNSMessage(host)).serialize();
        DatagramPacket request =
            new DatagramPacket(requestData, requestData.length, InetAddress.getByAddress(MDNS_ADDR), MDNS_PORT);
        multicastSocket.send(request);
    }

    // inter-process communication
    // poor man's message queue

    private Queue<Command> commandQueue = new ConcurrentLinkedQueue<Command>();
    private static abstract class Command {
    }
    private static class QuitCommand extends Command {}
    private static class QueryCommand extends Command {
        public QueryCommand(String host) { this.host = host; }
        public String host;
    }
    public void submitQuery(String host) {
        commandQueue.offer(new QueryCommand(host));
        if(multicastSocket!=null){
        	multicastSocket.close();
        }
    }
    public void submitQuit() {
        commandQueue.offer(new QuitCommand());
        if (multicastSocket != null) {
            multicastSocket.close();
        }
    }

}
