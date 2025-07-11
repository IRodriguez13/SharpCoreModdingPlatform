package com.example.examplemod;
public class GenericGameActionHandler implements InstructionHandler {
    @Override
    public void handle(InstructionPayload payload) throws Exception {
        // Ejemplo: Dar diamantes al jugador
        int cantidad = Integer.parseInt(payload.data.get("cantidad").toString());
        EntityPlayer player = Minecraft.getMinecraft().thePlayer;
        player.inventory.addItemStackToInventory(new ItemStack(Items.DIAMOND, cantidad));
    }
}

