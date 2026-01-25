using System.Numerics;
using CommandLine;
using LutViewer.Engine.Common;
using LutViewer.Engine.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces.Conversion;

namespace LutViewer.Engine;

public class Program
{
    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<AppArgs>(args).WithParsed(appArgs => { });

        Lut lut = LutUtil.ReadCubeFile(@"C:\Users\Thierry\DEV\cross\lutviewer\test\1\Lut.cube");
        Image<PixelFormat> img = ImageUtil.OpenImage(@"C:\Users\Thierry\DEV\cross\lutviewer\test\1\WithoutLut.JPG");
        LutUtil.ApplyLut(img, lut);
        img.SaveAsPng("processed.png");
    }
}
