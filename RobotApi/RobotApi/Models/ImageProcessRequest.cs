using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RobotApi.Models;

public class ImageProcessRequest
{
    [Required(ErrorMessage = "Image is required.")]
    [FromForm(Name = "image")]
    public IFormFile Image { get; set; }

    [Required(ErrorMessage = "Quality is required.")]
    [Range(16, 128, ErrorMessage = "Quality must be between 16 and 128.")]
    [FromForm(Name = "quality")]
    public int Quality { get; set; }
}
