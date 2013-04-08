//
//  AppDelegate.m
//  InfiniteStorage
//
//  Created by ben wei on 4/2/13.
//  Copyright (c) 2013 Waveface Inc. All rights reserved.
//

#import "AppDelegate.h"
#import "WFBonjourServer.h"


@implementation AppDelegate {
    WFBonjourServer * _server;
    BOOL _running;
}

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    _server = [[WFBonjourServer alloc] init];

  
    _server.port = 1338;
  
    [self startService];
}

- (BOOL) startService
{
    _running = [_server start];
    NSString * msg = [NSString stringWithFormat:@"Service started: %@.", _running ? @"OK" : @"Fail" ];
    [statusBar setStringValue:msg];
    actionButton.title = @"Stop";
    return _running;
};

- (void) stopService
{
    NSString * msg = [NSString stringWithFormat:@"Service stopped."];
    [statusBar setStringValue:msg];
    [_server stop];
   
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
