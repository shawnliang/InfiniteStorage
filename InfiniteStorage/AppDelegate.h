//
//  AppDelegate.h
//  InfinityStorage
//
//  Created by ben wei on 4/2/13.
//  Copyright (c) 2013 Waveface Inc. All rights reserved.
//

#import <Cocoa/Cocoa.h>

@interface AppDelegate : NSObject <NSApplicationDelegate> {
    IBOutlet NSTextField *statusBar;
    IBOutlet NSButton *actionButton;
}

@property (assign) IBOutlet NSWindow *window;


- (IBAction) actionClicked:(id)sender;


@end
