using System.Numerics;
using LutViewer.Engine.Common;
using SixLabors.ImageSharp;

namespace LutViewer.Engine.Util;

public static class LutUtil
{
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
}
