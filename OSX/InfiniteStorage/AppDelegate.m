//
//  AppDelegate.m
//  InfiniteStorage
//
//  Created by ben wei on 4/2/13.
//  Copyright (c) 2013 Waveface Inc. All rights reserved.
//

#import "AppDelegate.h"
#import "WFBonjourServer.h"
#import "WFWebSocketServer.h"

#define DEFAULT_SERVICE_PORT 1338

@implementation AppDelegate {
    WFBonjourServer * bonjourServer;
    WFWebSocketServer *wsServer;
    BOOL _running;
}

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    bonjourServer = [[WFBonjourServer alloc] init];
    
    wsServer = [[WFWebSocketServer alloc] init];
    wsServer.port = DEFAULT_SERVICE_PORT;
    bonjourServer.port = wsServer.port;

    [self startService];
}

- (BOOL) startService
{
    [wsServer start];

    _running = [bonjourServer start];
    NSString * msg = [NSString stringWithFormat:@"Service started: %@.", _running ? @"OK" : @"Fail" ];
    [statusBar setStringValue:msg];
    actionButton.title = @"Stop";
    return _running;
};

- (void) stopService
{
    NSString * msg = [NSString stringWithFormat:@"Service stopped."];
    [statusBar setStringValue:msg];
    [bonjourServer stop];
    [wsServer stop];
   
    actionButton.title = @"start";
    _running = NO;
}

- (void) onoffService
{
    if (_running) {
        [self stopService];
    } else {
        [self startService];
    }
}

- (IBAction) actionClicked:(id)sender
{
    [self onoffService];
}

@end
