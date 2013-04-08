//
//  SOBonjourServer.h
//  InfiniteStorage
//
//  Created by ben wei on 4/2/13.
//  Copyright (c) 2013 Waveface Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface WFBonjourServer : NSObject

@property (nonatomic, strong, readwrite) NSNetService *netService;
@property (nonatomic, assign) NSUInteger  port;

- (BOOL) start;
- (void) stop;
@end

