using System.Numerics;
using LutViewer.Engine.Common;
using SixLabors.ImageSharp;
using ZLinq;
using ZLinq.Linq;

namespace LutViewer.Engine.Util;

// https://kono.phpage.fr/images/a/a1/Adobe-cube-lut-specification-1.0.pdf
public static class LutUtil
{
    private const string LutSizeFalg = "LUT_3D_SIZE";

    public static void ApplyLut(Image<PixelFormat> image, Lut lut)
    {
        ImageUtil.MutatePerPixel(image, (ref pixel) => ApplyPixleLut(ref pixel, lut));
    }

    private static void ApplyPixleLut(ref PixelFormat pixel, Lut lut)
    {
        Vector3 colorVec3 = ImageUtil.PixelToVector3(pixel);
        Vector3 newColor = SampleLut(colorVec3, lut);
        ImageUtil.ApplyToPixel(newColor, ref pixel);
    }

    // This is from chatGPT. It solved wrong interpolation
    // Apparently this is called trilinearinterpolation
    private static Vector3 SampleLut(Vector3 c, Lut lut)
    {
        int N = lut.LutSize;
        float r = c.X * (N - 1);
        float g = c.Y * (N - 1);
        float b = c.Z * (N - 1);

        int r0 = (int)MathF.Floor(r);
        int g0 = (int)MathF.Floor(g);
        int b0 = (int)MathF.Floor(b);

        int r1 = Math.Min(r0 + 1, N - 1);
        int g1 = Math.Min(g0 + 1, N - 1);
        int b1 = Math.Min(b0 + 1, N - 1);

        float fr = r - r0;
        float fg = g - g0;
        float fb = b - b0;

        Vector3 c000 = lut.Data[r0 + g0 * N + b0 * N * N];
        Vector3 c100 = lut.Data[r1 + g0 * N + b0 * N * N];
        Vector3 c010 = lut.Data[r0 + g1 * N + b0 * N * N];
        Vector3 c110 = lut.Data[r1 + g1 * N + b0 * N * N];
        Vector3 c001 = lut.Data[r0 + g0 * N + b1 * N * N];
        Vector3 c101 = lut.Data[r1 + g0 * N + b1 * N * N];
        Vector3 c011 = lut.Data[r0 + g1 * N + b1 * N * N];
        Vector3 c111 = lut.Data[r1 + g1 * N + b1 * N * N];

        Vector3 c00 = Vector3.Lerp(c000, c100, fr);
        Vector3 c10 = Vector3.Lerp(c010, c110, fr);
        Vector3 c01 = Vector3.Lerp(c001, c101, fr);
        Vector3 c11 = Vector3.Lerp(c011, c111, fr);

        Vector3 c0 = Vector3.Lerp(c00, c10, fg);
        Vector3 c1 = Vector3.Lerp(c01, c11, fg);

        return Vector3.Lerp(c0, c1, fb);
    }

    public static Lut ReadCubeFile(string path)
    {
        ValueEnumerable<ArrayWhere<string>, string> content = File.ReadAllLines(path)
            .AsValueEnumerable()
            .Where(x => !x.StartsWith('#'));

        string lutSizeLine = content.FirstOrDefault(x => x.StartsWith(LutSizeFalg), string.Empty);
        if (
            lutSizeLine is null
            || lutSizeLine == string.Empty
            || !int.TryParse(lutSizeLine.Replace(LutSizeFalg, string.Empty), out int lutSize)
        )
        {
            throw new InvalidDataException(
                $"Could not find or parse line which specifies {LutSizeFalg} in the file {path}, {lutSizeLine}"
            );
        }
        Vector3[]? lut = content
            .Where(line => char.IsNumber(line[0]) || line[0] == '.')
            .Select(line =>
                line.Split(' ').AsValueEnumerable().Select(num => float.Parse(num)).ToArray()
            )
            .Where(x => x.Length == 3)
            .Select(line => new Vector3(line[0], line[1], line[2]))
            .ToArray();

        return lut is not null
            ? new Lut(lut, lutSize)
            : throw new InvalidDataException($"There was no valide value in the file {path}");
    }

    /// <summary>
    /// This function takes a rgb value and cinverts it into the lut array index. It is a float because it will be linearely interpolated
    /// </summary>
    /// <param name="r">Red value</param>
    /// <param name="g">Green value</param>
    /// <param name="b">Blue value</param>
    /// <param name="N">Lut Size, defined in the lut file with the LutSizeFalg </param>
    /// <returns></returns>
    public static float RGBToLutIndex(float r, float g, float b, int N) =>
        (N - 1) * (r + N * g + N * N * b);

    public static float Vector3ToLutIndex(Vector3 color, int N) =>
        RGBToLutIndex(color.X, color.Y, color.Z, N);
}
