using System.Reflection;
using MSharp.ModAPI;

namespace MSharp.Launcher.Core.ModRunner;

public static class ModLoader
{
    // -- ModLoader es una clase estática que se encarga de cargar mods desde una ruta específica Sólo funciona si uso mods compilados del lado del user --
    // CargarMods busca en la ruta especificada todos los archivos .dll,
    // intenta cargar cada uno como un ensamblado, y luego busca tipos que implementen
    // la interfaz IMsharpMod. Si encuentra alguno, lo instancia y lo agrega a la lista de mods.
    // Si hay errores durante la carga, los captura y los imprime en la consola.
    public static List<IMsharpMod> CargarMods(string ruta)
    {
        List<IMsharpMod> mods = [];

        if (!Directory.Exists(ruta))
        {
            Console.WriteLine($"📂 Carpeta de mods inexistente: {ruta}");
            return mods;
        }

        foreach (var archivo in Directory.GetFiles(ruta, "*.dll"))
        {
            try
            {
                Assembly? asm = Assembly.LoadFrom(archivo);
                IEnumerable<Type>? tipos = asm.GetTypes().Where(t => typeof(IMsharpMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var tipo in tipos)
                {
                    if (Activator.CreateInstance(tipo) is not IMsharpMod mod)
                    {
                        Console.WriteLine($"[ModCompiler] No se pudo instanciar: {tipo.FullName}");

                    }
                    else
                    {
                        mods.Add(mod);
                        Console.WriteLine($"[ModCompiler] Mod cargado: {tipo.FullName}");
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"[ModCompiler] Error de tipos en {archivo}: {ex.Message}");

                foreach (var loaderEx in ex.LoaderExceptions) if (loaderEx != null) Console.WriteLine($"[ModCompiler]   - {loaderEx.Message}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModCompiler] Error cargando {archivo}: {ex.Message}");
            }
        }

        return mods;
    }
}

