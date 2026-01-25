using CommandLine;
using LutViewer.Engine.Common;

namespace LutViewer.Engine;

public class Program
{
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<AppArgs>(args).WithParsed(appArgs => { });
    }
}
