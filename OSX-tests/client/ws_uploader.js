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
        that.ws.send(data, {mask: true});
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

var SOUploader = function (filename, wsclient) {
    this.filename = filename;
    this.wsclient = wsclient;
    this.bufferSize = 512;
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

    var req = {file: filename, size: stats.size};

    self.wsclient.send(JSON.stringify(req));

    fs.createReadStream(this.filename, {
        'flags': 'r',
        'encoding': 'binary',
        'mode': 0666,
        'bufferSize': self.bufferSize
    }).addListener( "data", function(chunk) {
        console.log("read and sent " + chunk.length + " bytes");
        self.wsclient.write(chunk);
    }).addListener( "close",function() {
        console.log("end of read");
        self.wsclient.send("end of file");
    });
});

SOUploader.method('upload', function () {
    var data = fs.readFileSync(this.filename);
    console.log("read " + data.length + " bytes from " + path.basename(this.filename));
    this.wsclient.write(data);
});

function main() {
    var client = wfclient();
    var ws_url = "ws://localhost:8080";

    var uploader = new SOUploader(__dirname + '/cherry_blossoms.JPG', client);

    client.onopen = function () {
        uploader.uploadStream();
    }

    client.connect(ws_url, function (err, data, ws, flags) {
        if (err) {
            console.log("data: " + err);
            return;
        }

        console.log("s->c flags:" + util.inspect(flags) + ",data " + data.length + " bytes");
    });

    return 0;
}

main()
