using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace ArtPixelisation;

public class PalettePixelation
{
    private readonly List<Rgba32> _palette;
    private readonly GridResizer _resizer;

    public PalettePixelation(List<Rgba32> palette, IResampler resampler)
    {
        _palette = palette;
        _resizer = new GridResizer(resampler);
    }

    public Image<Rgba32> Apply(Image<Rgba32> original, int targetSize)
    {
        var downscaled = _resizer.Resize(original, targetSize);

        for (int y = 0; y < downscaled.Height; y++)
        {
            for (int x = 0; x < downscaled.Width; x++)
            {
                var originalColor = downscaled[x, y];
                var nearestColor = FindNearestColor(originalColor);
                downscaled[x, y] = nearestColor;
            }
        }

        return downscaled;
    }

    private Rgba32 FindNearestColor(Rgba32 color)
    {
        float minDistance = float.MaxValue;
        Rgba32 nearest = default;

        foreach (var paletteColor in _palette)
        {
            float distance = GetColorDistance(color, paletteColor);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = paletteColor;
            }
        }

        return nearest;
    }

    private float GetColorDistance(Rgba32 c1, Rgba32 c2)
    {
        int dr = c1.R - c2.R;
        int dg = c1.G - c2.G;
        int db = c1.B - c2.B;
        return dr * dr + dg * dg + db * db;
    }
}