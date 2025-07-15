using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ShpCore.Logging;

namespace SharpCore.CLI.Env.FileManagement;

public static class SharpCoreHome
{
    public static string Root => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".sharpcore"
    );

    public static string KernelsDir => Path.Combine(Root, "kernels");
    public static string LogsDir => Path.Combine(Root, "logs");
    public static string ActiveKernelFile => Path.Combine(KernelsDir, "active.txt");
    public static string MetadataDir => Path.Combine(Root, "metadata");
    public static string VersionMetadataFile => Path.Combine(MetadataDir, "avalaible.json");
    public static string GetKernelDll(string version) =>  Path.Combine(KernelsDir, version, "sharpcore.kernel.dll");
    public static string ActiveKernelDll =>  GetKernelDll(File.ReadAllText(ActiveKernelFile).Trim());

    public static string[] GetInstalledVersions()
    {
        return Directory.GetDirectories(KernelsDir)
            .Select(d => new DirectoryInfo(d).Name)
            .Where(v => v != "active")
            .OrderDescending()
            .ToArray();
    }

    public static void EnsureStructure()
    {
        Directory.CreateDirectory(KernelsDir);
        Directory.CreateDirectory(LogsDir);
        Directory.CreateDirectory(MetadataDir);
    
            // Active kernel version
        if (!File.Exists(ActiveKernelFile))
                File.WriteAllText(ActiveKernelFile, VersionMetadataFile); // o dejarlo vacío si no hay una por defecto

        // Available versions metadata
        if (!File.Exists(VersionMetadataFile))
                File.WriteAllText(VersionMetadataFile, "[]"); // JSON vacío, lista de versiones
    }

    public static void AddVersionToMetadata(string version)
    {
        EnsureStructure();

        string file = VersionMetadataFile;
        List<string> versions = [];

        if (File.Exists(file))
        {
            string json = File.ReadAllText(file);
            versions = JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }

        if (!versions.Contains(version))
        {
            versions.Add(version);
            File.WriteAllText(file, JsonSerializer.Serialize(versions, new JsonSerializerOptions { WriteIndented = true }));
            KernelLog.Info($"✔ Versión {version} añadida al registro remoto local.");
        }
    }

}
