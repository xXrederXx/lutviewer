using System.Numerics;
using CommandLine;
using LutViewer.Engine.Common;
using LutViewer.Engine.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.Processing;

namespace LutViewer.Engine;

public class Program
{
    private static void Main(string[] args)
    {
        Parser
            .Default.ParseArguments<AppArgs>(args)
            .WithParsed(appArgs =>
            {
                Image<PixelFormat> img = OpenImage(appArgs.ImagePath);

                if (appArgs.ProcessSize > 0)
                {
                    img.Mutate(x => x.Resize(appArgs.ProcessSize, img.Height / img.Width * appArgs.ProcessSize));
                }

                string imageName = Path.GetFileName(appArgs.ImagePath);

                if (!Path.Exists(appArgs.OutputPath))
                    Directory.CreateDirectory(appArgs.OutputPath);

                foreach (string lutPath in GetLutPaths(appArgs.LutsPath))
                {
                    string lutName = Path.GetFileName(lutPath);
                    Image<PixelFormat> imgCopy = img.Clone();
                    Lut lut = LutUtil.ReadCubeFile(lutPath);

                    LutUtil.ApplyLut(imgCopy, lut);

                    img.SaveAsPng(Path.Combine(appArgs.OutputPath, $"{imageName}-{lutName}.png"));
                }
            });
    }

    private static Image<PixelFormat> OpenImage(string path)
    {
        return Path.Exists(path)
            ? ImageUtil.OpenImage(path)
            : throw new FileNotFoundException(
                "The file provided as image input could not be found.",
                path
            );
    }

    private static string[] GetLutPaths(string path) =>
        Directory.GetFiles(path, "*.cube", SearchOption.AllDirectories);
}
