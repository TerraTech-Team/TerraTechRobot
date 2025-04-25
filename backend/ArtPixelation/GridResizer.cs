using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace ArtPixelisation;

public static class GridResizer
{
    private static readonly IResampler resampler = KnownResamplers.Box;

    public static Image<Rgba32> Resize(Image<Rgba32> original, int targetSize)
    {
        var padded = CreatePaddedSquareImage(original);

        return padded.Clone(ctx => ctx.Resize(new ResizeOptions
        {
            Size = new Size(targetSize, targetSize),
            Sampler = resampler,
            Mode = ResizeMode.Stretch
        }));
    }

    private static Image<Rgba32> CreatePaddedSquareImage(Image<Rgba32> original)
    {
        var squareSize = Math.Max(original.Width, original.Height);
        var padded = new Image<Rgba32>(squareSize, squareSize, new Rgba32(0, 0, 0, 0));
        var offsetX = (squareSize - original.Width) / 2;
        var offsetY = (squareSize - original.Height) / 2;

        padded.Mutate(ctx => ctx.DrawImage(original, new Point(offsetX, offsetY), 1f));
        return padded;
    }
}