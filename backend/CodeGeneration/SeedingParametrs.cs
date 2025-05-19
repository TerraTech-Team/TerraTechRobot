using System;

namespace CodeGeneration
{
    public class SeedingParameters
    {
        public float AreaWidthCm { get; set; }
        public float AreaHeightCm { get; set; }

        public int PixelsX { get; set; }
        public int PixelsY { get; set; }
        public float StepX => AreaWidthCm / PixelsX;
        public float StepY => AreaHeightCm / PixelsY;
    }
}
