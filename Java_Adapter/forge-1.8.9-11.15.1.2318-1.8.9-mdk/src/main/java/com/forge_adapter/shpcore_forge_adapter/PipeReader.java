package com.forge_adapter.shpcore_forge_adapter;

import java.io.*;
import java.net.*;
import com.google.gson.Gson;

public class PipeReader {
    public static void start() {
        try (ServerSocket serverSocket = new ServerSocket(12345)) {
            while (true) {
                try (Socket clientSocket = serverSocket.accept();
                     BufferedReader in = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));
                     PrintWriter out = new PrintWriter(clientSocket.getOutputStream(), true)) {

                    String json = in.readLine();
                    InstructionPayload payload = new Gson().fromJson(json, InstructionPayload.class);

                    try {
                        InstructionRegister.handle(payload);
                        out.println("ok");
                    } catch (Exception e) {
                        out.println("fail");
                    }
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
