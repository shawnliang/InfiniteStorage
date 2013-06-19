/**
 *
 */
package com.waveface.jmdns.impl;

import java.net.InetAddress;

import com.waveface.jmdns.JMDNS;
import com.waveface.jmdns.NetworkTopologyEvent;
import com.waveface.jmdns.NetworkTopologyListener;

/**
 * @author C&eacute;drik Lime, Pierre Frisch
 */
public class NetworkTopologyEventImpl extends NetworkTopologyEvent implements Cloneable {

    /**
     *
     */
    private static final long serialVersionUID = 1445606146153550463L;

    private final InetAddress _inetAddress;

    /**
     * Constructs a Network Topology Event.
     * 
     * @param jMDNS
     * @param inetAddress
     * @exception IllegalArgumentException
     *                if source is null.
     */
    public NetworkTopologyEventImpl(JMDNS jMDNS, InetAddress inetAddress) {
        super(jMDNS);
        this._inetAddress = inetAddress;
    }

    NetworkTopologyEventImpl(NetworkTopologyListener jmmDNS, InetAddress inetAddress) {
        super(jmmDNS);
        this._inetAddress = inetAddress;
    }

    /*
     * (non-Javadoc)
     * @see javax.jmdns.NetworkTopologyEvent#getDNS()
     */
    @Override
    public JMDNS getDNS() {
        return (this.getSource() instanceof JMDNS ? (JMDNS) getSource() : null);
    }

    /*
     * (non-Javadoc)
     * @see javax.jmdns.NetworkTopologyEvent#getInetAddress()
     */
    @Override
    public InetAddress getInetAddress() {
        return _inetAddress;
    }

    @Override
    public String toString() {
        StringBuilder buf = new StringBuilder();
        buf.append("[" + this.getClass().getSimpleName() + "@" + System.identityHashCode(this) + " ");
        buf.append("\n\tinetAddress: '");
        buf.append(this.getInetAddress());
        buf.append("']");
        // buf.append("' source: ");
        // buf.append("\n\t" + source + "");
        // buf.append("\n]");
        return buf.toString();
    }

    /*
     * (non-Javadoc)
     * @see java.lang.Object#clone()
     */
    @Override
    public NetworkTopologyEventImpl clone() throws CloneNotSupportedException {
        return new NetworkTopologyEventImpl(getDNS(), getInetAddress());
    }

}
