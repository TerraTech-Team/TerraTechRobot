using System.ComponentModel.DataAnnotations;

namespace RobotApi.Models;

public class CodeGenerateRequest
{
    public IFormFile? Image { get; set; }
        
    [Range(1, 10000, ErrorMessage = "Length must be between 1 and 10000.")]
    public int? Length { get; set; }
    
    [Range(1, 10000, ErrorMessage = "Width must be between 1 and 10000.")]
    public int? Width { get; set; }
}