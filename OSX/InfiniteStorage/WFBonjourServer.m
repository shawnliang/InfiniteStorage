//
//  SOBonjourServer.m
//  InfiniteStorage
//
//  Created by ben wei on 4/2/13.
//  Copyright (c) 2013 Waveface Inc. All rights reserved.
//

#import "WFBonjourServer.h"
#include "SOIPAddress.h"

@implementation WFBonjourServer {}

- (BOOL) start
{
    self.netService = [[NSNetService alloc] initWithDomain:@"local" type:@"_infinite-storage._tcp." name:@"" port:(int) self.port];
    NSMutableDictionary*txtRecordDataDictionary = [[NSMutableDictionary alloc ] init];
    char mac_addr[30] = {0};
    int r = SO_get_mac_address(mac_addr, sizeof(mac_addr) - 1, "en0");
    NSString *serverID = [NSString stringWithFormat:@"fixed-servercode"];

    if (r > 0) {
        serverID = [NSString stringWithFormat:@"%s", mac_addr];
    }
    NSString *ServerIDHash = [NSString stringWithFormat:@"%016lx", (unsigned long) [serverID hash]];
    NSLog(@"hash(%@) ==> %@", serverID, ServerIDHash);
    [txtRecordDataDictionary setValue:[NSString stringWithFormat:@"1.0"] forKey:@"version"];
    [txtRecordDataDictionary setValue:[NSString stringWithFormat:@"%lu", self.port] forKey:@"ws_port"];
    [txtRecordDataDictionary setObject:ServerIDHash forKey:@"server_id"];
   
    [self.netService setTXTRecordData:[NSNetService dataFromTXTRecordDictionary:txtRecordDataDictionary]];
    [self.netService publishWithOptions:0];

    return YES;
}

- (void)stop
{
    [self.netService stop];
    self.netService = nil;
}

@end
