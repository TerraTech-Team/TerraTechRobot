using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace ArtPixelisation;

public static class GridResizer
{
    private static readonly IResampler resampler = KnownResamplers.Box;

    public static Image<Rgba32> Resize(Image<Rgba32> original, int quality)
    {
        int newWidth, newHeight;

        if (original.Width >= original.Height)
        {
            newWidth = quality;
            newHeight = (int)Math.Round(original.Height * (quality / (double)original.Width));
        }
        else
        {
            newHeight = quality;
            newWidth = (int)Math.Round(original.Width * (quality / (double)original.Height));
        }

        return original.Clone(ctx => ctx.Resize(new ResizeOptions
        {
            Size = new Size(newWidth, newHeight),
            Sampler = resampler,
            Mode = ResizeMode.Stretch
        }));
    }
}

