using System.Numerics;
using SixLabors.ImageSharp;

namespace LutViewer.Engine.Util;

public static class ImageUtil
{
    public delegate void ActionRef<T>(ref T item);

    public static Image<PixelFormat> OpenImage(string path)
    {
        return Image.Load<PixelFormat>(path);
    }

    public static void MutatePerPixel(Image<PixelFormat> image, ActionRef<PixelFormat> action)
    {
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<PixelFormat> pixelRow = accessor.GetRowSpan(y);

                for (int x = 0; x < pixelRow.Length; x++)
                {
                    // Get a reference to the pixel at position x
                    action.Invoke(ref pixelRow[x]);
                }
            }
        });
    }

    public static Vector3 PixelToVector3(in PixelFormat pixel)
    {
        return new Vector3(pixel.R / (float)byte.MaxValue, pixel.G / (float)byte.MaxValue, pixel.B / (float)byte.MaxValue);
    }
    public static void ApplyToPixel(in Vector3 newPixel, ref PixelFormat pixel)
    {
        pixel.R = (byte)(newPixel.X * byte.MaxValue);
        pixel.G = (byte)(newPixel.Y * byte.MaxValue);
        pixel.B = (byte)(newPixel.Z * byte.MaxValue);
    }
}
