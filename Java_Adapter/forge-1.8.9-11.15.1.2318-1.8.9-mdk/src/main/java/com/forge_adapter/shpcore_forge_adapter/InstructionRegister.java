package com.forge_adapter.shpcore_forge_adapter;
import java.util.Map;
import java.util.HashMap;

public class InstructionRegister{
    
    private static final Map<String, InstructionHandler> handlers = new HashMap<>();

    public static void register(String tipo, InstructionHandler handler) {
        if(tipo == null) throw new IllegalArgumentException("Type argument is null");
        handlers.put(tipo, handler);
    }

    public static void handle(InstructionPayload payload) throws Exception {
        InstructionHandler handler = handlers.get(payload.tipo);
        if (handler == null) throw new IllegalArgumentException("Instruction type is null");
        handler.handle(payload);
    }
}

