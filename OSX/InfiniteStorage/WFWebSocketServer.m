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

#define OP_TEXT   0x1
#define OP_BINARY 0x2
#define WS_MODE_FILE 1
#define WS_MODE_STANDBY 0

@implementation WFWebSocketServer {
    NSThread *wsThread;
    NSMutableDictionary *status;
    BOOL running;
    NSMutableDictionary *sessions;
    NSString *savePath;
}

static WFWebSocketServer *sharedWSServer = nil;

- (id) init
{
    self = [super init];
    if (self) {
        sharedWSServer = self;
        sessions = [[NSMutableDictionary alloc] init];
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

static void websocket_ready_handler(struct mg_connection *conn) {
    char buf[40];
    size_t n = snprintf((char *) buf, sizeof(buf) - 1, "%s", "{\"banner\":\"server ready\"}");
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

- (NSDictionary *) JSONToDic: (const char *) data withLength: (size_t) len
{
    NSError *error;
    NSData *payload = [NSData dataWithBytes:data length:len];
    NSDictionary *res = [NSJSONSerialization JSONObjectWithData:payload options:NSJSONReadingMutableContainers error:&error];
    return res;
}

- (void) createSession: (const char *) data withSize: (size_t) data_len withFlags: (int) flags withIndex: (unsigned long) index
{
    NSString *sessionID = [NSString stringWithFormat:@"%lu", index];
    //NSDictionary *sess = [sessions objectForKey: sessionID];
    NSMutableDictionary *sess = [sessions objectForKey:@"sessionID"];
    NSDictionary *res = [sharedWSServer JSONToDic:data withLength:data_len];
    if (res == nil) {
        NSLog(@"Error: JSON Parse failure [%@]rcv[%lu]: f:%08x : %.*s\n", sessionID, data_len, flags, (int) data_len, data);
        return;
    }
    
    [sess setObject:res forKey:@"res"];
    NSString *filename = [res objectForKey:@"file_name"] ;
    if (filename) {
        [sess setValue:filename forKey:@"filename"];
    }
    
    NSString *action = [res objectForKey:@"action"];
    NSNumber *wsMode = nil;
    if ([action isEqualToString:@"file-start"]) {
        sess = [[NSMutableDictionary alloc] init];
        wsMode = [NSNumber numberWithInt:WS_MODE_FILE];
        [sess setValue:wsMode forKey:@"ws_mode"];
    } else if ([action isEqualToString:@"file-end"]) {
        wsMode = [NSNumber numberWithInt:WS_MODE_STANDBY];
        [sess setValue:wsMode forKey:@"ws_mode"];
        [self closeOutputStreamWithID: sessionID];
        NSLog(@"close file [%@]rcv[%lu]: f:%08x : %.*s\n", sessionID, data_len, flags,(int) data_len, data);
        return;
    } else {
        NSLog(@"unknown [%@]rcv[%lu]: f:%08x : %.*s\n", sessionID, data_len, flags,(int) data_len, data);
        return;
    }

    NSLog(@"[%@] createSession...", sessionID);
    NSString *size = [res objectForKey:@"file_size"] ;
    size_t totalSize = 0;
    if (size) {
        totalSize = [size intValue];
        [sess setValue:[NSNumber numberWithLong:totalSize] forKey:@"total_size"];
    }

    NSNumber *fileLen = [NSNumber numberWithLong:0];
    [sess setValue:fileLen forKey:@"file_len"];

    NSLog(@"[%@]rcv[%lu]: f:%08x file:%@ size:%lu from %.*s\n", sessionID, data_len, flags, filename, totalSize, (int) data_len, data);
 
     NSString *pathname = [NSString stringWithFormat:@"%@/%@", savePath, filename];
    [sess setObject:pathname forKey:@"target_pathname"];
    NSOutputStream *oStream = [[NSOutputStream alloc] initToFileAtPath:pathname append:YES];
    [sess setObject:oStream forKey:@"output_stream"];
    [oStream open];
    [sessions setObject:sess forKey:sessionID];
}

- (NSMutableDictionary *) getSession:(unsigned long) index
{
    NSString *sessionID = [NSString stringWithFormat:@"%lu", index];
    NSMutableDictionary *sess = [sessions objectForKey:sessionID];
    return sess;
}

- (BOOL) isFileUploading: (unsigned long) index {
    NSDictionary *sess = [self getSession: index];
    NSNumber *wsMode = [sess objectForKey:@"ws_mode"];
    return WS_MODE_FILE == [wsMode integerValue];
}

/*
 * @return YES, match file total size, otherwise NO is return;
 */

- (long) writeOutputStream: (const uint8_t *) data withSize: (size_t) size withIndex: (unsigned long) index
{
    NSString *sessionID = [NSString stringWithFormat:@"%lu", index];
    NSMutableDictionary *sess = [sessions objectForKey:sessionID];
    NSOutputStream *oStream = [sess objectForKey:@"output_stream"];

    [oStream write:data maxLength:size];

    NSNumber *fileLen = [sess objectForKey:@"file_len"];
    long len = [fileLen longValue] + size;
    [sess setValue: [NSNumber numberWithLong:len] forKey:@"file_len"];
    long total = [[sess objectForKey:@"total_size"] longValue];
    NSLog(@"%@ rcvb[%lu]: %lu/%lu\n", sessionID, size, len, total);
    if (len == total) {
        [sess setValue: [NSNumber numberWithLong:WS_MODE_STANDBY] forKey:@"ws_mode"];
        return YES;
    }
    return NO;
}

- (void) closeOutputStreamWithID: (NSString *) sessionID
{
    NSMutableDictionary *sess = [sessions objectForKey:sessionID];
    NSOutputStream *oStream = [sess objectForKey:@"output_stream"];
    [oStream close];
    oStream = nil;
}

- (void) closeOutputStream: (unsigned long) index
{
    NSString *sessionID = [NSString stringWithFormat:@"%lu", index];
    NSMutableDictionary *sess = [sessions objectForKey:sessionID];
    NSOutputStream *oStream = [sess objectForKey:@"output_stream"];
    [oStream close];
    oStream = nil;
}

int wfReplyWithCode(struct mg_connection *conn, int code)
{
    char buf[40];
    size_t n = snprintf((char *) buf, sizeof(buf) - 1,
                        "{\"action\", \"file-end\",\"code\": %d}", code);
    return mg_websocket_write(conn, OP_TEXT, buf, n);
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
            if (*data == '{' && (data[1] == '\'' || data[1] == '\"')) {
                [sharedWSServer createSession: data withSize: data_len withFlags: flags withIndex:(unsigned long) conn];
            } else {
                NSLog(@"rcv[%lu]: f:%08x %.*s\n", data_len, flags, (int) data_len, data);
            }
            break;
        case OP_BINARY:
            if ([sharedWSServer isFileUploading:(unsigned long) conn]) {
                BOOL ret = [sharedWSServer writeOutputStream:(const uint8_t *) data withSize:data_len withIndex:(unsigned long) conn];
                if (ret) {
                    wfReplyWithCode(conn, 0);
                }
            } else {
                NSLog(@"rcvb[%lu]: f:%08x\n", data_len, flags);
            }
            break;
        default:
            NSLog(@"rcv[%lu]: f:%08x\n", data_len, flags);            
    }
#ifdef WS_DEBUG
    websocket_reply_received_length(conn, data_len);
#endif
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
    NSFileManager *fileManager= [NSFileManager defaultManager];
    savePath = [NSString stringWithFormat:@"%@/Desktop/InfiniteStorage", NSHomeDirectory()];
    BOOL isDir = FALSE;
    if(![fileManager fileExistsAtPath:savePath isDirectory:&isDir]) {
        if(![fileManager createDirectoryAtPath:savePath withIntermediateDirectories:YES attributes:nil error:NULL]) {             NSLog(@"Error: Create folder failed %@", savePath);
        }
    }
    [wsThread start];
    
    return FALSE;
}

- (void) stop
{
    running = false;
}


@end
