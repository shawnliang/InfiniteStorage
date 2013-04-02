//
//  SOBonjourServer.m
//  InfiniteStorage
//
//  Created by ben wei on 4/2/13.
//  Copyright (c) 2013 Waveface Inc. All rights reserved.
//

#import "WFBonjourServer.h"

@implementation WFBonjourServer {}

- (BOOL) start
{
    self.netService = [[NSNetService alloc] initWithDomain:@"local" type:@"_infinite-storage._tcp." name:@"" port:(int) self.port];
    NSMutableDictionary*txtRecordDataDictionary = [[NSMutableDictionary alloc ] init];
    
    [txtRecordDataDictionary setValue:[NSString stringWithFormat:@"1.0"] forKey:@"version"];
    [txtRecordDataDictionary setValue:[NSString stringWithFormat:@"%lu", self.port] forKey:@"ws_port"];
    NSLog(@"%@: %@", self.netService, txtRecordDataDictionary);
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
