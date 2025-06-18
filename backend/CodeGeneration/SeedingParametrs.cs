using System;

namespace CodeGeneration
{
    public class SeedingParameters
    {
        public float AreaWidthCm { get; set; }
        public float AreaHeightCm { get; set; }

        public int PixelsX { get; set; }
        public int PixelsY { get; set; }

        private const int motorStepsInCm = 800; // этот параметр меняется эмпирически в зависимости от моторчика
        public float StepX => AreaWidthCm / PixelsX * motorStepsInCm;
        public float StepY => AreaHeightCm / PixelsY * motorStepsInCm;
    }
}
