using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace ArtPixelisation;

public class GridResizer
{
    private readonly IResampler _resampler;

    public GridResizer(IResampler? resampler = null)
    {
        _resampler = resampler ?? KnownResamplers.Box;
    }

    public Image<Rgba32> Resize(Image<Rgba32> original, int targetSize)
    {
        if (targetSize <= 0 || (targetSize & (targetSize - 1)) != 0)
            throw new ArgumentException("Размер должен быть положительной степенью двойки: 16, 32, 64 и т.д.", nameof(targetSize));

        int squareSize = Math.Max(original.Width, original.Height);

        var padded = new Image<Rgba32>(squareSize, squareSize, new Rgba32(0, 0, 0, 0));
        int offsetX = (squareSize - original.Width) / 2;
        int offsetY = (squareSize - original.Height) / 2;
        padded.Mutate(ctx => ctx.DrawImage(original, new Point(offsetX, offsetY), 1f));

        return padded.Clone(ctx => ctx.Resize(new ResizeOptions
        {
            Size = new Size(targetSize, targetSize),
            Sampler = _resampler,
            Mode = ResizeMode.Stretch
        }));
    }
}