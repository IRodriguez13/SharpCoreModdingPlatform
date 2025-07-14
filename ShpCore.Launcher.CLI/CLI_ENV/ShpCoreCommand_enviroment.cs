using System.CommandLine;
using System;
using System.IO;
using ShpCore.Logging;
using System.Threading.Tasks;
using SharpCore.Kernel.Init;
using System.Runtime.InteropServices;
using SharpCore.CLI.Env.FileManagement;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;


// USO DE EJEMPLO: sharpcore run --protocol namedpipe --adapter forge --payload ./mods/axel.json

namespace SharpCore.CLI.Env;

public static class SharpCoreCLI
{
    public static async Task Run(string[] args)
    {
        SharpCoreHome.EnsureStructure(); // <- Se asegura que todo exista

        RootCommand root = new("CLI oficial de SharpCore") { Name = "shpcore" }; SharpCoreHome.EnsureStructure(); // <- Se asegura que todo exista

        // Muestra el banner y la información del núcleo ni bien se inicia el CLI en color blanco y amarillo
        Console.ForegroundColor = ConsoleColor.White;
        CoreFecth();
        Console.ResetColor();


        Option<string> protocolOption = new Option<string>("--protocol", "Protocolo de transporte")
        {
            IsRequired = false
        }.FromAmong("namedpipe", "grpc", "unix");

        Option<string> adapterOption = new("--adapter", "Ruta al adaptador")
        {
            IsRequired = false
        };

        Command kernelCheck = new("-kernel-check", "Consulta si hay una nueva versión disponible");

        kernelCheck.SetHandler(() =>
        {
            // Simula lectura desde metadata
            var localVersions = SharpCoreHome.GetInstalledVersions();
            var availableJson = File.ReadAllText(SharpCoreHome.VersionMetadataFile);
            var availableVersions = JsonSerializer.Deserialize<List<string>>(availableJson);

            if (availableVersions == null || availableVersions.Count == 0)
            {
                KernelLog.Warn("No hay versiones remotas disponibles registradas.");
                return;
            }

            var latest = availableVersions.OrderByDescending(v => v).FirstOrDefault();
            var installed = localVersions.Contains(latest);

            if (!installed)
            {
                KernelLog.Warn($"🔔 Hay una nueva versión disponible: {latest}");
                return;
            }

            KernelLog.Info($"✔ Ya tenés la última versión ({latest}) instalada.");
        });



        Command kernelRollback = new("-kernel-rollback", "Vuelve a la versión anterior del kernel (si existe)");

        kernelRollback.SetHandler(() =>
        {
            string activePath = SharpCoreHome.ActiveKernelFile.Trim();
            string currentVersion = File.ReadAllText(activePath).Trim();
            var versionPath = Path.Combine(SharpCoreHome.KernelsDir, currentVersion);
            string[] installedVersions = SharpCoreHome.GetInstalledVersions();

            var currentIndex = Array.IndexOf(installedVersions, currentVersion);
            if (currentIndex == -1 || currentIndex + 1 >= installedVersions.Length)
            {
                KernelLog.Warn("No hay versiones anteriores disponibles.");
                return;
            }
            string previous = installedVersions[currentIndex + 1];
            File.WriteAllText(activePath, previous);
            KernelLog.Info($"✔ Volviste a la versión anterior: {previous}");



        });

        Option<string> localPathOption = new("--path", "Ruta a un kernel local ya compilado") { IsRequired = true };
        Option<string> localVersionOption = new("--version", "Versión a registrar") { IsRequired = true };

        Command kernelAddLocal = new("kernel-add-local", "Registra un kernel compilado localmente");
        kernelAddLocal.AddOption(localPathOption);
        kernelAddLocal.AddOption(localVersionOption);

        kernelAddLocal.SetHandler((string path, string version) =>
        {
            string kernelsDir = Path.Combine(SharpCoreHome.ActiveKernelDll, "kernels");
            string targetDir = Path.Combine(kernelsDir, version);

            if (!Directory.Exists(path))
            {
                KernelLog.Warn($"❌ El path {path} no existe.");
                return;
            }

            if (Directory.Exists(targetDir))
            {
                KernelLog.Warn($"❌ Ya existe un kernel registrado como {version}.");
                return;
            }

            Directory.CreateDirectory(targetDir);
            File.Copy(Path.Combine(path, "sharpcore.kernel.dll"), Path.Combine(targetDir, "sharpcore.kernel.dll"));

            KernelLog.Info($"✔ Kernel local registrado como versión {version}.");
        }, localPathOption, localVersionOption);



        Command kernelList = new("kernel-list", "Lista los kernels instalados localmente");
        kernelList.SetHandler(() =>
        {
            string kernelsPath = Path.Combine(SharpCoreHome.ActiveKernelFile, "kernels");
            string active = File.ReadAllText(Path.Combine(kernelsPath, "active.txt")).Trim();

            foreach (var dir in Directory.GetDirectories(kernelsPath))
            {
                string version = new DirectoryInfo(dir).Name;
                string label = version == active ? " (active)" : "";
                KernelLog.Info($"✔ {version}{label}");
            }
        });


        Option<string> switchVersionOption = new("--version", "Versión a activar") { IsRequired = true };

        Command kernelSwitch = new("kernel-switch", "Activa otra versión del kernel");
        kernelSwitch.AddOption(switchVersionOption);
        kernelSwitch.SetHandler((string version) =>
        {
            string versionPath = Path.Combine(SharpCoreHome.KernelsDir, "kernels", version);
            if (!Directory.Exists(versionPath))
            {
                KernelLog.Warn($"La versión {version} no está instalada.");
                return;
            }

            File.WriteAllText(Path.Combine(SharpCoreHome.MetadataDir, "kernels", "active.txt"), version);
            KernelLog.Info($"✔ Versión activa cambiada a {version}");
        }, switchVersionOption);

        Command kernelUpdate = new("kernel-update", "Descarga la última versión del kernel");
        kernelUpdate.SetHandler(() =>
        {
            KernelLog.Info("✔ Simulando descarga desde remoto...");
            // Lógica real: descargar zip, extraer a ~/.sharpcore/kernels/vX.Y.Z/
        });


        Command neofetch = new("corefetch", "Muestra información del núcleo SharpCore");

        neofetch.SetHandler(() =>
        {
            CoreFecth();

            string version = File.ReadAllText("VERSION.txt").Trim();
            string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
                     RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS" : "Unknown";

            var arch = RuntimeInformation.OSArchitecture;
            string runtime = RuntimeInformation.FrameworkDescription;

            Console.ForegroundColor = ConsoleColor.Yellow;

            KernelLog.Info($"Kernel version: {version}");
            KernelLog.Info("Author: Iván Rodriguez (ivanr013) <ivanrwcm25@gmail.com>");
            KernelLog.Info($"Runtime: {os} {arch} / {runtime}");
            KernelLog.Info("GitHub: https://github.com/IRodriguez13/SharpCore_forge");
            KernelLog.Info("Adapter: No selected (use --adapter /path/to/adapter)");
            KernelLog.Info("ASCII font: Banner3 (logo), 3x5 (version)");
            KernelLog.Info("License: GPL-3.0");

            Console.ResetColor();
        });


        Command HelpCommand = new("--utils", "Muestra la ayuda del CLI");
        HelpCommand.AddAlias("--u");
        HelpCommand.AddAlias("--U");

        HelpCommand.SetHandler(() =>
        {
            KernelLog.Info("[CLI] Uso de SharpCore CLI:");
            ShowUtils();
        });

        Command runCommand = new("run", "Ejecuta un payload contra el núcleo");
        runCommand.AddOption(protocolOption);
        runCommand.AddOption(adapterOption);

        Option<string> payloadOption = new("--payload", "Ruta al archivo JSON con el payload") { IsRequired = true };
        runCommand.AddOption(payloadOption);

        runCommand.SetHandler((string payloadPath, string protocol, string adapterPath) =>
        {
            if (!File.Exists(payloadPath))
            {
                KernelLog.Panic($"(PAYLOAD) El archivo {payloadPath} no existe.");
                return;
            }

            if (!Directory.Exists(adapterPath))
            {
                KernelLog.Panic($"(ADAPTER) La ruta del adaptador '{adapterPath}' no existe. Asegurate de clonar el adaptador.");
                return;
            }

            try
            {
                SharpCoreKernel.Run(payloadPath, protocol, adapterPath);
            }
            catch (Exception ex)
            {
                KernelLog.Panic($"Error al ejecutar SharpCore: {ex.Message}");
            }
        }, payloadOption, protocolOption, adapterOption);


        root.AddCommand(runCommand);
        root.AddCommand(HelpCommand);
        root.AddCommand(neofetch);
        root.AddCommand(kernelList);
        root.AddCommand(kernelSwitch);
        root.AddCommand(kernelUpdate);

        await root.InvokeAsync(args);
    }

    private static void ShowUtils()
    {
        Console.WriteLine(
        @"SharpCore CLI - Uso básico

        Comandos:
        --protocol    [namedpipe|grpc|unix]        Protocolo de transporte
        --adapter     [forge|gba|ps2]              Adaptador (consola/juego destino)
        --payload     path/to/payload.json         Ruta del archivo de instrucción

        Ejemplo:
        sharpcore run --protocol namedpipe --adapter forge --payload ./mods/axel.json"
        );
    }

    private static void CoreFecth()
    {
        string banner = File.ReadAllText("Banner.txt");
        Console.WriteLine(banner);
    }

}


