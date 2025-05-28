using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using CodeGeneration;

namespace RobotApi.Services
{
    public class SeedingPreparationService
    {
        private readonly Dictionary<Rgba32, int> paletteMap;

        public SeedingPreparationService()
        {
            paletteMap = GetPaletteMap();
        }

        public async Task<(SeedingParameters parameters, int[,] colorMap)> PrepareAsync(
            Stream imageStream,
            SeedingParameters parameters)
        {
            using var image = await Image.LoadAsync<Rgba32>(imageStream);

            parameters.PixelsX = image.Width;
            parameters.PixelsY = image.Height;

            var colorMap = GetColorMap(image);
            return (parameters, colorMap);
        }

        private int[,] GetColorMap(Image<Rgba32> image)
        {
            int width = image.Width;
            int height = image.Height;
            var map = new int[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel = image[x, y];
                    map[y, x] = paletteMap.TryGetValue(pixel, out var containerId) ? containerId : -1;
                }
            }

            return map;
        }

        private Dictionary<Rgba32, int> GetPaletteMap() => new()
        {
            [new Rgba32(220, 20, 60)] = 0,
            [new Rgba32(255, 140, 0)] = 1,
            [new Rgba32(255, 215, 0)] = 2,
            [new Rgba32(60, 179, 113)] = 3,
            [new Rgba32(100, 149, 237)] = 4,
            [new Rgba32(186, 85, 211)] = 5,
            [new Rgba32(245, 245, 245)] = 6,
            [new Rgba32(30, 30, 30)] = 7,
            [new Rgba32(255, 182, 193)] = 8,
            [new Rgba32(139, 69, 19)] = 9
        };
    }
}
