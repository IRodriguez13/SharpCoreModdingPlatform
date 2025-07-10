namespace MSharp.ModAPI
{
    // -- This is the universal interface for SharpCore Mods --
    public interface IMsharpMod
    {
        void OnStart();
        void OnTick();
        void OnEvent(string evnt, object? payload = null);
    }
}
