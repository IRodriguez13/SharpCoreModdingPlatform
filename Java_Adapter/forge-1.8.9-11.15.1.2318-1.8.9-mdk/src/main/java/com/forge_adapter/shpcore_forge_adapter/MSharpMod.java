package com.forge_adapter.shpcore_forge_adapter;

import cpw.mods.fml.common.Mod;
import cpw.mods.fml.common.event.FMLInitializationEvent;

import com.forge_adapter.shpcore_forge_adapter.InstructionRegister;
import com.forge_adapter.shpcore_forge_adapter.GenericGameActionHandler;
import com.forge_adapter.shpcore_forge_adapter.PipeReader;

@Mod(modid = "msharp", version = "1.0", name = "MSharp Mod")
public class MSharpMod {
    @Mod.EventHandler
    public void init(FMLInitializationEvent event) {
        InstructionRegister.register("game", new GenericGameActionHandler());

        // Acá se abre la conexión al pipe, se escucha, se parsea y se llama a InstructionRegister.handle(...)
        PipeReader.start();
    }
}

 