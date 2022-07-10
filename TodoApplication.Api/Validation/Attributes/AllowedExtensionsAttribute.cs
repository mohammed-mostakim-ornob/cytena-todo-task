using System.ComponentModel.DataAnnotations;

namespace TodoApplication.Api.Validation.Attributes;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;
    
    private new string ErrorMessage => $"This photo extension is not allowed.";
    
    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions.Select(x => x.ToLower()).ToArray();
    }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not IFormFile file)
            return ValidationResult.Success;
        
        var extension = Path.GetExtension(file.FileName).Replace(".", "");
        
        return !_extensions.Contains(extension.ToLower()) ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
    }
}