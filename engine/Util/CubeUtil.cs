using System.Numerics;
using ZLinq;

namespace LutViewer.Engine.Util;

public static class CubeUtil
{
    private const string LutSizeFalg = "LUT_3D_SIZE";

    public static Vector3[] ReadCubeFile(string path)
    {
        var content = File.ReadAllLines(path).AsValueEnumerable().Where(x => !x.StartsWith('#'));

        string lutSizeLine = content.FirstOrDefault(x => x.StartsWith(LutSizeFalg), string.Empty);
        if (
            lutSizeLine is null
            || lutSizeLine == string.Empty
            || !int.TryParse(lutSizeLine, out int lutSize)
        )
        {
            throw new InvalidDataException(
                $"Could not find or parse line which specifies {LutSizeFalg} in the file {path}"
            );
        }
        var lut = content
            .Where(line => char.IsNumber(line[0]) || line[0] == '.')
            .Select(line =>
                line.Split(' ').AsValueEnumerable().Select(num => float.Parse(num)).ToArray()
            )
            .Where(x => x.Length == 3)
            .Select(line => new Vector3(line[0], line[1], line[2]))
            .ToArray();

        return lut is not null
            ? lut
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
    public static float RGBToLutIndex(float r, float g, float b, int N) => r + N * g + N * N * b;
}
