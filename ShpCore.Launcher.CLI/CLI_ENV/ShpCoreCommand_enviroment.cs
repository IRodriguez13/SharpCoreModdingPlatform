using System.CommandLine;
using System;
using System.IO;
using ShpCore.Logging;
using System.Threading.Tasks;
using SharpCore.Kernel.Init;


// USO DE EJEMPLO: sharpcore run --protocol namedpipe --adapter forge --payload ./mods/axel.json

namespace SharpCore.CLI.Env;

public static class SharpCoreCLI
{
    public static async Task Run(string[] args)
    {
        CoreFecth();

        RootCommand root = new("CLI oficial de SharpCore")
        {
            Name = "shpcore"
        };

        var protocolOption = new Option<string>("--protocol", "Protocolo de transporte")
        {
            IsRequired = false
        }.FromAmong("namedpipe", "grpc", "file");

        var adapterOption = new Option<string>("--adapter", "Adaptador de plataforma")
        {
            IsRequired = false
        }.FromAmong("forge", "gba", "ps2");

        Command neofetch = new("corefetch", "Muestra información del núcleo SharpCore");
                neofetch.SetHandler(() =>
                {
                    CoreFecth();
                    KernelLog.Info("Versión: " + File.ReadAllText("VERSION.txt").Trim());
                    KernelLog.Info("Autor: Ivan Rodriguez (ivanr013)");
                    KernelLog.Info("GitHub: https://github.com/IRodriguez13/SharpCore_forge");
                    KernelLog.Info("Distro: SharpCore Forge");
                }
            );

        Command versionCommand = new("--version", "Muestra la versión actual del núcleo SharpCore");
        versionCommand.AddAlias("-v");
        versionCommand.AddAlias("-V");
        versionCommand.SetHandler(() =>
        {
            var version = File.ReadAllText("VERSION.txt").Trim();
            KernelLog.Info($"[CLI] SharpCore v {version}");
        });


        Command HelpCommand = new("--utils", "Muestra la ayuda del CLI");
        HelpCommand.AddAlias("-u");
        HelpCommand.AddAlias("-U");

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

        runCommand.SetHandler((string payloadPath, string protocol, string adapter) =>
        {
            if (!File.Exists(payloadPath))
            {
                KernelLog.Panic($"El archivo {payloadPath} no existe.");
                return;
            }

            try
            {
                SharpCoreKernel.Run(payloadPath, protocol, adapter);
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
        --protocol    [namedpipe|grpc|file]        Protocolo de transporte
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


