//
//  WFWebSocketServer.h
//  InfiniteStorage
//
//  Created by ben wei on 4/8/13.
//  Copyright (c) 2013 Waveface Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface WFWebSocketServer : NSObject
@property (nonatomic, assign) NSUInteger  port;

- (BOOL) start;
- (void) stop;

+ (WFWebSocketServer *) shared;

@end
