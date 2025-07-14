using System.CommandLine;
using System;
using System.IO;
using ShpCore.Logging;
using System.Threading.Tasks;
using SharpCore.Kernel.Init;
using System.Runtime.InteropServices;


// USO DE EJEMPLO: sharpcore run --protocol namedpipe --adapter forge --payload ./mods/axel.json

namespace SharpCore.CLI.Env;

public static class SharpCoreCLI
{
    public static async Task Run(string[] args)
    {
        CoreFecth();

        RootCommand root = new("CLI oficial de SharpCore")
        {
            Name = "sharpcore"
        };

        var protocolOption = new Option<string>("--protocol", "Protocolo de transporte")
        {
            IsRequired = false
        }.FromAmong("namedpipe", "grpc", "unix");

        var adapterOption = new Option<string>("--adapter", "Ruta al adaptador")
        {
            IsRequired = false
        };


        Command neofetch = new("corefetch", "Muestra información del núcleo SharpCore");

        neofetch.SetHandler(() =>
        {
            CoreFecth();

            var version = File.ReadAllText("VERSION.txt").Trim();
            var os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
                     RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS" : "Unknown";

            var arch = RuntimeInformation.OSArchitecture;
            var runtime = RuntimeInformation.FrameworkDescription;
           
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


        Command versionCommand = new("--version", "Muestra la versión actual del núcleo SharpCore");
        versionCommand.AddAlias("-v");
        versionCommand.AddAlias("-V");
        versionCommand.SetHandler(() =>
        {
            var version = File.ReadAllText("VERSION.txt").Trim();
            KernelLog.Info($"[CLI] SharpCore v {version}");
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

        var payloadOption = new Option<string>("--payload", "Ruta al archivo JSON con el payload") { IsRequired = true };
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


        root.AddCommand(versionCommand);
        root.AddCommand(runCommand);

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
        var banner = File.ReadAllText("Banner.txt");
        Console.WriteLine(banner);
    }

}


