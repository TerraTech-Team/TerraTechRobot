using System;
using System.Threading;
using System.Diagnostics;

namespace CodeGeneration
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Starting robot firmware...");

            try
            {
                var robot = Robot.Create();
                var codeGenerator = new CodeGenerator();

                var parameters = new SeedingParameters
                {
                    AreaWidthCm = GeneratedData.AreaWidthCm,
                    AreaHeightCm = GeneratedData.AreaHeightCm,
                    PixelsX = GeneratedData.PixelsX,
                    PixelsY = GeneratedData.PixelsY
                };

                codeGenerator.ExecutePlanting(
                    robot,
                    GeneratedData.ColorMap,
                    parameters.PixelsX,
                    parameters.PixelsY,
                    parameters
                );

                Debug.WriteLine("Planting completed successfully!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
