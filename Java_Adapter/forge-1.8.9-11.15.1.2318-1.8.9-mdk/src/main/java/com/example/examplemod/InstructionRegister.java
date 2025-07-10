
public class InstructionRegister 
{
    private static final Map<String, InstructionHandler> handlers = new HashMap<>();

    public static void register(String tipo, InstructionHandler handler) {
        handlers.put(tipo, handler);
    }

    public static void handle(InstructionPayload payload) throws Exception {
        InstructionHandler handler = handlers.get(payload.tipo);
        if (handler == null) throw new IllegalArgumentException("Tipo de instrucción no soportado");
        handler.handle(payload);
    }
}

