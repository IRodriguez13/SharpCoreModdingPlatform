using ShpCore.Logging;
using MSharp.Launcher.Core;
using SharpCore.CLI.Env;
using System.Threading.Tasks;

namespace ShpCore.Launcher.CLI;

public class Program
{
    public static async void Main(string[] args) => await SharpCoreCLI.Run(args);

}
