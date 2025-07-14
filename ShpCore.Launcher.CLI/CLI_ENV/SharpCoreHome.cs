using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

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

    public static string[] GetInstalledVersions()
    {
        return Directory.GetDirectories(KernelsDir)
            .Select(d => new DirectoryInfo(d).Name)
            .Where(v => v != "active")
            .OrderDescending()
            .ToArray();

    }


    public static string GetKernelDll(string version) =>
        Path.Combine(KernelsDir, version, "sharpcore.kernel.dll");

    public static string ActiveKernelDll =>
        GetKernelDll(File.ReadAllText(ActiveKernelFile).Trim());

    public static void EnsureStructure()
    {
        Directory.CreateDirectory(KernelsDir);
        Directory.CreateDirectory(LogsDir);
        Directory.CreateDirectory(MetadataDir);
    }
}
