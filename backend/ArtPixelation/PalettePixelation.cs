using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ArtPixelisation;

public static class PalettePixelation
{
    public static PixelationResult Apply(List<Rgba32> palette, Image<Rgba32> original, int targetSize)
    {
        var downscaled = GridResizer.Resize(original, targetSize);
        var usedColors = new HashSet<Rgba32>();

        Parallel.For(0, downscaled.Height,
            () => new HashSet<Rgba32>(),
            (y, _, localSet) =>
            {
                var row = downscaled.DangerousGetPixelRowMemory(y).Span;
                for (var x = 0; x < downscaled.Width; x++)
                {
                    var nearest = FindNearestColor(palette, row[x]);
                    row[x] = nearest;
                    localSet.Add(nearest);
                }
                return localSet;
            },
            localSet =>
            {
                lock (usedColors)
                {
                    foreach (var color in localSet)
                        usedColors.Add(color);
                }
            });

        return new PixelationResult(downscaled, usedColors.ToList());
    }


    private static Rgba32 FindNearestColor(List<Rgba32> palette, Rgba32 color)
    {
        var minDistance = float.MaxValue;
        Rgba32 nearest = default;

        foreach (var paletteColor in palette)
        {
            var distance = GetColorDistance(color, paletteColor);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = paletteColor;
            }
        }

        return nearest;
    }

    private static float GetColorDistance(Rgba32 c1, Rgba32 c2)
    {
        var v1 = new Vector3(c1.R, c1.G, c1.B);
        var v2 = new Vector3(c2.R, c2.G, c2.B);
        return Vector3.DistanceSquared(v1, v2);
    }
}