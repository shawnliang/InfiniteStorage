//
//  WFWebSocketServer.m
//  InfiniteStorage
//
//  Created by ben wei on 4/8/13.
//  Copyright (c) 2013 Waveface Inc. All rights reserved.
//

#import "WFWebSocketServer.h"

#include <stdio.h>
#include <string.h>
#include "mongoose.h"
#include <arpa/inet.h>



@implementation WFWebSocketServer {
    NSThread *wsThread;
    NSMutableDictionary *status;
    BOOL running;
    NSOutputStream *oStream;
}

static WFWebSocketServer *sharedWSServer = nil;

- (id) init
{
    self = [super init];
    if (self) {
        sharedWSServer = self;
    }
    return self;
}

+ (WFWebSocketServer *) shared
{
    if (!sharedWSServer) {
        sharedWSServer = [[WFWebSocketServer alloc] init];
    }
    return sharedWSServer;
}

#define OP_TEXT   0x1
#define OP_BINARY 0x2

static void websocket_ready_handler(struct mg_connection *conn) {
    char buf[40];
    size_t n = snprintf((char *) buf, sizeof(buf) - 1, "%s", "server ready");
    mg_websocket_write(conn, OP_TEXT, buf, n);
}


static int websocket_reply_received_length(struct mg_connection *conn, size_t data_len)
{
    char reply[200];
    size_t *pointToReply;
    pointToReply = (size_t *) (reply);
    *pointToReply = htonl(data_len);
    return mg_websocket_write(conn, OP_BINARY, reply, 4);
}

static size_t filelen = 0;
#define WS_MODE_FILE 1
#define WS_MODE_STANDBY 0
static int ws_mode = WS_MODE_STANDBY;
static size_t totalSize = 0;

- (NSDictionary *) JSONToDic: (const char *) data withLength: (size_t) len
{
    NSError *error;
    NSData *payload = [NSData dataWithBytes:data length:len];
    NSDictionary *res = [NSJSONSerialization JSONObjectWithData:payload options:NSJSONReadingMutableContainers error:&error];
    return res;
}

- (void) createOutputStream: (NSString *) filename
{
   NSLog(@"Creating and opening NSOutputStream...");
   NSString *pathname = [NSString stringWithFormat:@"%@/Desktop/%@", NSHomeDirectory(), filename];
   // oStream is an instance variable
   oStream = [[NSOutputStream alloc] initToFileAtPath:pathname append:YES];
//   [oStream scheduleInRunLoop:[NSRunLoop currentRunLoop]
//                           forMode:NSDefaultRunLoopMode];
   [oStream open];
}

- (void) writeOutputStream: (const uint8_t *) data withSize: (size_t) size
{
    [oStream write:data maxLength:size];
}

- (void) closeOutputStream
{
    [oStream close];
    oStream = nil;
}

// Arguments:
//   flags: first byte of websocket frame, see websocket RFC,
//          http://tools.ietf.org/html/rfc6455, section 5.2
//   data, data_len: payload data. Mask, if any, is already applied.
static int websocket_data_handler(struct mg_connection *conn, int flags,
                                  char *data, size_t data_len) {
    (void) flags;
    unsigned char op = 0;
    op = flags & 0xf;

    switch (op) {
        case OP_TEXT:
            if (memcmp(data, "{\"file\":", 8) == 0) {
                NSDictionary *res = [sharedWSServer JSONToDic:data withLength:data_len];
                NSString *filename = [res objectForKey:@"file"] ;
                NSString *size = [res objectForKey:@"size"] ;
                totalSize = [size intValue];
                filelen = 0;
                ws_mode = WS_MODE_FILE;
                [sharedWSServer createOutputStream: filename];
                NSLog(@"rcv[%lu]: f:%08x file:%@ size:%lu\n", data_len, flags, filename, totalSize);
            } else {
                NSLog(@"rcv[%lu]: f:%08x %s\n", data_len, flags, data);
            }
            break;
        case OP_BINARY:
            if (ws_mode == WS_MODE_FILE) {
                filelen += data_len;
                [sharedWSServer writeOutputStream:(const uint8_t *) data withSize:data_len];
                NSLog(@"rcvb[%lu]: f:%08x: %lu/%lu\n", data_len, flags, filelen, totalSize);
                if (filelen == totalSize) {
                    [sharedWSServer closeOutputStream];
                    ws_mode = WS_MODE_STANDBY;
                    char buf[40];
                    size_t n = snprintf((char *) buf, sizeof(buf) - 1, "recv eof: total: %lu", filelen);
                    mg_websocket_write(conn, OP_TEXT, buf, n);
                }
            } else {
                NSLog(@"rcvb[%lu]: f:%08x\n", data_len, flags);
            }
            break;
        default:
            NSLog(@"rcv[%lu]: f:%08x\n", data_len, flags);            
    }
    
    websocket_reply_received_length(conn, data_len);
    return 1;
}


-(void) startWSThread:(NSMutableDictionary *) status {

    const char *strPort = [[NSString stringWithFormat:@"%lu", _port] cStringUsingEncoding:NSASCIIStringEncoding];
        
    const char *strDocRoot = [[NSString stringWithFormat:@"%@", [[NSBundle mainBundle] resourcePath]] cStringUsingEncoding:NSUTF8StringEncoding];
    NSLog(@"WS Thread Started %s\n%s", strPort, strDocRoot);
    struct mg_context *ctx;
    struct mg_callbacks callbacks;
    const char *options[] = {
        "listening_ports", strPort,
        "document_root", strDocRoot,
        NULL
    };

    memset(&callbacks, 0, sizeof(callbacks));
    callbacks.websocket_ready = websocket_ready_handler;
    callbacks.websocket_data = websocket_data_handler;
    ctx = mg_start(&callbacks, NULL, options);

    while(running) {
        sleep(1);
    }
    mg_stop(ctx);

    NSLog(@"WS Thread stopped");
}

- (BOOL) start
{
    running = true;
    wsThread = [[NSThread alloc] initWithTarget:self selector:@selector(startWSThread:) object:status];
    
    [wsThread start];
    
    return FALSE;
}

- (void) stop
{
    running = false;
}


@end
