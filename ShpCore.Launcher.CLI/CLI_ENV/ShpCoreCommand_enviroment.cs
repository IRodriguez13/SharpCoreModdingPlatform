using System.CommandLine;
using MSharp.Launcher.Core;
using MSharp.Launcher.Core.Bridge;
using System;
using MSharp.Launcher.Core.Models;
using System.Text.Json;
using System.IO;
using ShpCore.Logging;
using System.Threading.Tasks;


// USO DE EJEMPLO: sharpcore run --protocol namedpipe --adapter forge --payload ./mods/axel.json

namespace SharpCore.CLI.Env;

public static class SharpCoreCLI
{
    public static async Task Run(string[] args)
    {
        var root = new RootCommand("CLI oficial de SharpCore");

        var protocolOption = new Option<string>("--protocol", "Protocolo de transporte") { IsRequired = false };
        var adapterOption = new Option<string>("--adapter", "Adaptador de plataforma") { IsRequired = false };


        var versionCommand = new Command("--version", "Muestra la versión actual del núcleo");
        versionCommand.SetHandler(() =>
        {
            var version = File.ReadAllText("VERSION.txt").Trim();
            KernelLog.Info($"[CLI] SharpCore v {version}");
        });

        var HelpCommand = new Command("--help", "Muestra la ayuda del CLI");
        HelpCommand.SetHandler(() =>
        {
            KernelLog.Info("[CLI] Uso de SharpCore CLI:");
            ShowHelp();

        }
        );

        var runCommand = new Command("run", "Ejecuta un payload contra el núcleo");
        runCommand.AddOption(protocolOption);
        runCommand.AddOption(adapterOption);
        var payloadOption = new Option<string>("--payload", "Ruta al archivo JSON con el payload") { IsRequired = true };
        runCommand.AddOption(payloadOption);

        runCommand.SetHandler((string payloadPath, string protocol, string adapter) =>
        {
            SharpCoreKernel_init.Run(payloadPath, protocol, adapter);
        }, payloadOption, protocolOption, adapterOption);

        root.AddCommand(versionCommand);
        root.AddCommand(runCommand);

        await root.InvokeAsync(args);
    }

    private static void ShowHelp()
    {
        Console.WriteLine(@"
SharpCore CLI - Uso básico

Comandos:
  --protocol    [namedpipe|grpc|file]        Protocolo de transporte
  --adapter     [forge|gba|ps2]              Adaptador (consola/juego destino)
  --payload     path/to/payload.json         Ruta del archivo de instrucción

Ejemplo:
  sharpcore run --protocol namedpipe --adapter forge --payload ./mods/axel.json
");
    }

}


