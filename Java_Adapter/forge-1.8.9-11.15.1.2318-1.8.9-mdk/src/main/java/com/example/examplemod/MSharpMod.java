
@Mod(modid = "msharp", version = "1.0", name = "MSharp Mod")
public class MSharpMod {
    @Mod.EventHandler
    public void init(FMLInitializationEvent event) {
        InstructionRegister.register("game", new GenericGameActionHandler());

        // Acá se abre la conexión al pipe, se escucha, se parsea y se llama a InstructionRegister.handle(...)
        PipeReader.start();
    }
}

