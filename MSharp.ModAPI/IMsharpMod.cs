namespace MSharp.ModAPI
{
    // -- Esta es la interfaz a implementar por todas las distrubuciones que moddeen con SharpCore --
    public interface IMsharpMod
    {
        void OnStart();
        void OnTick();
        void OnEvent(string evnt, object? payload = null);
    }
}
