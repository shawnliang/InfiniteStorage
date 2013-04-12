//
//  SOIPAddress.c
// source: https://gist.github.com/benwei/5369096
/*
 Copyright (c) 2013, Ben Wei
 All rights reserved.
 
 Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 
 Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
 Neither the name of the StarOS nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
 THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

#include <stdio.h>
#include <errno.h>
#include <sys/types.h>
#include <stdio.h>
#include <string.h>
#include <sys/socket.h>
#include <net/if_dl.h>
#include <ifaddrs.h>

#if ! defined(IFT_ETHER)
#define IFT_ETHER 0x6/* Ethernet CSMACD */
#endif

int SO_get_mac_address(char* mac_addr, size_t maclen, char* ifname) {
    int  ret;
    struct ifaddrs * addrs;
    struct ifaddrs * cursor;
    const struct sockaddr_dl * dlAddr;
    const unsigned char* base;
    int i, currsize = 0;
    
    ret = getifaddrs(&addrs);
    if (ret != 0) {
        return ret;
    }

    for (cursor = addrs; cursor != NULL ; cursor = cursor->ifa_next) {
        if ( (cursor->ifa_addr->sa_family == AF_LINK)
            && (((const struct sockaddr_dl *) cursor->ifa_addr)->sdl_type == IFT_ETHER)
            && strcmp(ifname,  cursor->ifa_name)==0 ) {

            dlAddr = (const struct sockaddr_dl *) cursor->ifa_addr;
            base = (const unsigned char*) &dlAddr->sdl_data[dlAddr->sdl_nlen];
            strcpy(mac_addr, "");
            for (i = 0; i < dlAddr->sdl_alen; i++) {
                if (i != 0) {
                    if (currsize < maclen) {
                        strcat(mac_addr, ":");
                        currsize++;
                    } else {
                        return -E2BIG;
                    }
                }
                char partialAddr[3];
                sprintf(partialAddr, "%02X", base[i]);
                if (currsize < maclen - 3) {
                    strcat(mac_addr, partialAddr);
                    currsize += 2;
                } else {
                    return -E2BIG;
                }
            }
            break;
        }
    }
    
    freeifaddrs(addrs);

    mac_addr[currsize] = '\0';
    return currsize;
}