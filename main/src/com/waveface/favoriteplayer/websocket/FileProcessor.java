package com.waveface.favoriteplayer.websocket;

import org.codehaus.jackson.map.ObjectMapper;
import org.jwebsocket.api.WebSocketPacket;
import org.jwebsocket.kit.RawPacket;
import org.jwebsocket.token.Token;

public class FileProcessor {
	public static WebSocketPacket tokenToPacket(Token aToken) {
		WebSocketPacket lPacket = null;
		try {
			ObjectMapper lMapper = new ObjectMapper();
			byte[] lData = lMapper.writeValueAsBytes(aToken.getMap().get("byte"));
			lPacket = new RawPacket(lData);
		} catch (Exception lEx) {
		}
		return lPacket;
	}

	public static Token packetToToken(WebSocketPacket aPacket) {
		// TODO Auto-generated method stub
		return null;
	}

}
