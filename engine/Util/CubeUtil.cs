using System.Numerics;
using LutViewer.Engine.Common;
using ZLinq;
using ZLinq.Linq;

namespace LutViewer.Engine.Util;
// https://kono.phpage.fr/images/a/a1/Adobe-cube-lut-specification-1.0.pdf
public static class CubeUtil
{
    private const string LutSizeFalg = "LUT_3D_SIZE";

    public static Lut ReadCubeFile(string path)
    {
        ValueEnumerable<ArrayWhere<string>, string> content = File.ReadAllLines(path).AsValueEnumerable().Where(x => !x.StartsWith('#'));

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
