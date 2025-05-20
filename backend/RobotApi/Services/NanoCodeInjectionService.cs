using RobotApi.Models;
using System.Text;

namespace RobotApi.Services
{
    public class NanoCodeInjectionService
    {
        public string GenerateFirmwareDataFile(int[,] colorMap, SeedingParameters parameters, string firmwareProjectPath)
        {
            var sb = new StringBuilder();

            sb.AppendLine("namespace CodeGeneration");
            sb.AppendLine("{");
            sb.AppendLine("    public static class GeneratedData");
            sb.AppendLine("    {");

            // ColorMap - правильное формирование двумерного массива
            sb.AppendLine("        public static readonly int[][] ColorMap = new int[][]");
            sb.AppendLine("        {");

            for (int y = 0; y < colorMap.GetLength(0); y++)
            {
                sb.Append("            new int[] { ");
                for (int x = 0; x < colorMap.GetLength(1); x++)
                {
                    sb.Append(colorMap[y, x]);
                    if (x < colorMap.GetLength(1) - 1)
                        sb.Append(", ");
                }
                sb.AppendLine(" },");
            }

            sb.AppendLine("        };");


            // Parameters
            sb.AppendLine($"        public static readonly float AreaWidthCm = {parameters.AreaWidthCm}f;");
            sb.AppendLine($"        public static readonly float AreaHeightCm = {parameters.AreaHeightCm}f;");
            sb.AppendLine($"        public static readonly int PixelsX = {parameters.PixelsX};");
            sb.AppendLine($"        public static readonly int PixelsY = {parameters.PixelsY};");

            sb.AppendLine("    }");
            sb.AppendLine("}");

            // Сохраняем в проект nanoFramework
            var filePath = Path.Combine(firmwareProjectPath, "GeneratedData.cs");
            File.WriteAllText(filePath, sb.ToString());
            return filePath;
        }
    }
}