using System.Text;

namespace RobotControl.CodeGeneration;

public class NanoFrameworkCodeGenerator
{
    public string Generate(List<Action> actions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("public class SeedingProgram");
        sb.AppendLine("{");
        sb.AppendLine("    public void Run()");
        sb.AppendLine("    {");
        sb.AppendLine("        // Code generated below");
        foreach (var action in actions)
        {
            // Симуляция. В реальности нужно сохранять описание действия
            sb.AppendLine("        // action();");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }
}