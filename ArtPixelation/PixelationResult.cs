using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ArtPixelisation;

public record PixelationResult(Image<Rgba32> Image, IReadOnlyCollection<Rgba32> UsedColors);