#!/usr/bin/env node
var util = require('util'),
    fs = require('fs'),
    path = require('path'),
    WebSocket = require('ws');

require('./lib/Method');


var verbose = true;

function wfclient() {
    var that = {}
    that.onopen = function () {};
    that.onend = function () {};

    that.write = function (data) {
        var uint8Packet = new Uint8Array(data.length);
        for(var i = 0; i < data.length ; i++) {
          uint8Packet[i] = data.charCodeAt(i);
        }
        that.ws.send(uint8Packet, {binary: true, mask: true});
    }

    that.send = function (data) {
        return that.ws.send(data, {binary: false, mask: false});
    }

    that.connect = function (target, cb) {
        var ws = new WebSocket(target,
                {protocolVersion: 13, origin: target});

        that.ws = ws;
        that.cb = cb;
        ws.on('error', function(err) {
            if (err.code == "ECONNREFUSED") {
                console.log("unable to connect to server: " + target);
            } else {
                console.log("ws error: " + util.inspect(err));
            }
        });

        ws.on('open', function() {
            console.log("ws open");
            that.onopen();
        });

        ws.on('close', function(code, data) {
            console.log("ws close ("+ code +") data " + data.length + " bytes.");
        });

        ws.on('message', function(data, flags) {
            that.cb(null, data, ws, flags)
        });
    }

    return that;
}

function shouldTerminate(ws) {
    ws.terminate();
    process.exit(0);
}

var SOUploader = function (filename, wsclient, bufferSize) {
    this.filename = filename;
    this.wsclient = wsclient;

    if (bufferSize < 512)
      bufferSize = 512

    this.bufferSize = bufferSize;
}

SOUploader.method('uploadStream', function () {
    var self = this;
    var filename = path.basename(this.filename);
    console.log("upload Stream from " + filename);

    var fs = require('fs');
    try {
        stats = fs.lstatSync(this.filename);
    } catch (e){
        console.log("except: " + util.inspect(e));
        return;
    }

    var req = {
      action: "file-start",
      file_name: filename,
      file_size: stats.size};

    self.wsclient.send(JSON.stringify(req));
    var blkno = 0;
    fs.createReadStream(this.filename, {
        'flags': 'r',
        'encoding': 'binary',
        'mode': 0666,
        'bufferSize': self.bufferSize
    }).addListener( "data", function(chunk) {
        blkno++;
        self.wsclient.write(chunk);
        console.log(blkno + ": sent " + chunk.length + " bytes");
    }).addListener( "close",function() {
        console.log("end of read");
        var endblob = {
            action: 'file-end',
            file_name: filename
        };
        setTimeout(function (){
          self.wsclient.send(JSON.stringify(endblob));
        }, 1000);
    });
});

SOUploader.method('upload', function () {
    var data = fs.readFileSync(this.filename);
    console.log("read " + data.length + " bytes from " + path.basename(this.filename));
    this.wsclient.write(data);
});

var syntax = function () {
  console.log("syntax: " + process.argv[1] + " <option> args\n"
    + " option: \n"
    + "url <ws_url>   -- with url\n"
    + "port <ws_port> -- with local port\n");
  process.exit(1);
}

function main() {
    var client = wfclient();
    var ws_url = "ws://localhost:1338";
    var port = 1338;
    var bufferSize = 0;

    if (process.argv.length > 3) {
        op = process.argv[2];
        if (op == 'url') {
          ws_url = process.argv[3];
        } else if (op == 'port') {
          ws_url = "ws://localhost:" + port;
        } else if (op == 'size') {
          bufferSize = parseInt(process.argv[3]);
        }
    }

    var filename = __dirname + '/cherry_blossoms.JPG';
    // var filename = __dirname + '/test.txt';

    var uploader = new SOUploader(filename, client, bufferSize);

    client.onopen = function () {
        uploader.uploadStream();
    }
    console.log("connecting to " + ws_url);
    client.connect(ws_url, function (err, data, ws, flags) {
        if (err) {
            console.log("data: " + err);
            return;
        }

        if (flags.binary == true) {
            if (data.length == 4) {
                console.log("s->c recv len:" +flags.buffer.readUInt32BE(0));
            } else {
            console.log("s->c flags:" + util.inspect(flags) + ",data " + data.length + " bytes");
            }
        } else {
            console.log("s->c data " + data.length + " bytes: " + data);
        }
    });

    return 0;
}

main()
