// Copyright 2003-2005 Arthur van Hoff, Rick Blair
// Licensed under Apache License version 2.0
// Original license LGPL

package com.waveface.jmdns.impl;

import java.io.IOException;
import java.net.DatagramPacket;
import java.util.logging.Level;
import java.util.logging.Logger;

import com.waveface.jmdns.impl.constants.DNSConstants;

/**
 * Listen for multicast packets.
 */
class SocketListener extends Thread {
    static Logger           logger = Logger.getLogger(SocketListener.class.getName());

    /**
     *
     */
    private final JMDNSImplement _jmDNSImpl;

    /**
     * @param jMDNSImplement
     */
    SocketListener(JMDNSImplement jMDNSImplement) {
        super("SocketListener(" + (jMDNSImplement != null ? jMDNSImplement.getName() : "") + ")");
        this.setDaemon(true);
        this._jmDNSImpl = jMDNSImplement;
    }

    @Override
    public void run() {
        try {
            byte buf[] = new byte[DNSConstants.MAX_MSG_ABSOLUTE];
            DatagramPacket packet = new DatagramPacket(buf, buf.length);
            while (!this._jmDNSImpl.isCanceling() && !this._jmDNSImpl.isCanceled()) {
                packet.setLength(buf.length);
                this._jmDNSImpl.getSocket().receive(packet);
                if (this._jmDNSImpl.isCanceling() || this._jmDNSImpl.isCanceled() || this._jmDNSImpl.isClosing() || this._jmDNSImpl.isClosed()) {
                    break;
                }
                try {
                    if (this._jmDNSImpl.getLocalHost().shouldIgnorePacket(packet)) {
                        continue;
                    }

                    DNSIncoming msg = new DNSIncoming(packet);
                    if (logger.isLoggable(Level.FINEST)) {
                        logger.finest(this.getName() + ".run() JmDNS in:" + msg.print(true));
                    }
                    if (msg.isQuery()) {
                        if (packet.getPort() != DNSConstants.MDNS_PORT) {
                            this._jmDNSImpl.handleQuery(msg, packet.getAddress(), packet.getPort());
                        }
                        this._jmDNSImpl.handleQuery(msg, this._jmDNSImpl.getGroup(), DNSConstants.MDNS_PORT);
                    } else {
                        this._jmDNSImpl.handleResponse(msg);
                    }
                } catch (IOException e) {
                    logger.log(Level.WARNING, this.getName() + ".run() exception ", e);
                }
            }
        } catch (IOException e) {
            if (!this._jmDNSImpl.isCanceling() && !this._jmDNSImpl.isCanceled() && !this._jmDNSImpl.isClosing() && !this._jmDNSImpl.isClosed()) {
                logger.log(Level.WARNING, this.getName() + ".run() exception ", e);
                this._jmDNSImpl.recover();
            }
        }
        if (logger.isLoggable(Level.FINEST)) {
            logger.finest(this.getName() + ".run() exiting.");
        }
    }

    public JMDNSImplement getDns() {
        return _jmDNSImpl;
    }

}
