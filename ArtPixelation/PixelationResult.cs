using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ArtPixelisation;

public class PixelationResult
{
    public Image<Rgba32> Image { get; }
    public IReadOnlyCollection<Rgba32> UsedColors { get; }

    public PixelationResult(Image<Rgba32> image, IReadOnlyCollection<Rgba32> usedColors)
    {
        Image = image;
        UsedColors = usedColors;
    }
}