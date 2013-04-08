// Copyright (c) 2004-2012 Sergey Lyubka
// This file is a part of mongoose project, http://github.com/valenok/mongoose

#include <stdio.h>
#include <string.h>
#include "mongoose.h"
#include <arpa/inet.h>

static void websocket_ready_handler(struct mg_connection *conn) {
  unsigned char buf[40];
  buf[0] = 0x81;
  buf[1] = snprintf((char *) buf + 2, sizeof(buf) - 2, "%s", "server ready");
  mg_write(conn, buf, 2 + buf[1]);
}

// Arguments:
//   flags: first byte of websocket frame, see websocket RFC,
//          http://tools.ietf.org/html/rfc6455, section 5.2
//   data, data_len: payload data. Mask, if any, is already applied.
static int websocket_data_handler(struct mg_connection *conn, int flags,
                                  char *data, size_t data_len) {
  unsigned char reply[200];
  size_t *pointToReply;
  (void) flags;

  if (flags & 2) {
      printf("rcvb[%lu]: f:%08x\n", data_len, flags);
  } else {
      printf("rcv: f:%08x: [%.*s]\n", flags, (int) data_len, data);
  }

  reply[0] = 0x82;  // binary, FIN set
  reply[1] = 4;
  pointToReply = (size_t *) (reply + 2);
  *pointToReply = htonl(data_len);

  mg_write(conn, reply, 6);

  return 1;
}

int main(void) {
  struct mg_context *ctx;
  struct mg_callbacks callbacks;
  const char *options[] = {
    "listening_ports", "8080",
    "document_root", "websocket_html_root",
    NULL
  };

  memset(&callbacks, 0, sizeof(callbacks));
  callbacks.websocket_ready = websocket_ready_handler;
  callbacks.websocket_data = websocket_data_handler;
  ctx = mg_start(&callbacks, NULL, options);
  int c;
  printf("Hit 'q' to quit.\n");
  while(1) {
      c = getchar();
      if (c == 'q') {
          printf("terminated.\n");
          break;
      }
  }
  mg_stop(ctx);

  return 0;
}
