// Copyright 2003-2005 Arthur van Hoff, Rick Blair
// Licensed under Apache License version 2.0
// Original license LGPL

package com.waveface.jmdns.impl.tasks;

import java.util.Timer;
import java.util.logging.Level;
import java.util.logging.Logger;


import com.waveface.jmdns.impl.JMDNSImplement;
import com.waveface.jmdns.impl.constants.DNSConstants;

/**
 * Periodically removes expired entries from the cache.
 */
public class RecordReaper extends DNSTask {
    static Logger logger = Logger.getLogger(RecordReaper.class.getName());

    /**
     * @param jMDNSImplement
     */
    public RecordReaper(JMDNSImplement jMDNSImplement) {
        super(jMDNSImplement);
    }

    /*
     * (non-Javadoc)
     * @see javax.jmdns.impl.tasks.DNSTask#getName()
     */
    @Override
    public String getName() {
        return "RecordReaper(" + (this.getDns() != null ? this.getDns().getName() : "") + ")";
    }

    /*
     * (non-Javadoc)
     * @see javax.jmdns.impl.tasks.DNSTask#start(java.util.Timer)
     */
    @Override
    public void start(Timer timer) {
        if (!this.getDns().isCanceling() && !this.getDns().isCanceled()) {
            timer.schedule(this, DNSConstants.RECORD_REAPER_INTERVAL, DNSConstants.RECORD_REAPER_INTERVAL);
        }
    }

    @Override
    public void run() {
        if (this.getDns().isCanceling() || this.getDns().isCanceled()) {
            return;
        }
        if (logger.isLoggable(Level.FINEST)) {
            logger.finest(this.getName() + ".run() JmDNS reaping cache");
        }

        // Remove expired answers from the cache
        // -------------------------------------
        this.getDns().cleanCache();
    }

}