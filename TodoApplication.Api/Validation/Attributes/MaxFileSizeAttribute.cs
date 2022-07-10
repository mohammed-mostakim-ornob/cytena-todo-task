using System.ComponentModel.DataAnnotations;

namespace TodoApplication.Api.Validation.Attributes;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly double _maxFileSizeInMb;

    private new string ErrorMessage => $"Maximum allowed file size is {_maxFileSizeInMb} MB.";

    public MaxFileSizeAttribute(double maxFileSizeInMb)
    {
        _maxFileSizeInMb = maxFileSizeInMb;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return value switch
        {
            null => ValidationResult.Success,
            IFormFile file when file.Length > _maxFileSizeInMb * 1024 * 1024 => new ValidationResult(ErrorMessage),
            _ => ValidationResult.Success
        };
    }
}