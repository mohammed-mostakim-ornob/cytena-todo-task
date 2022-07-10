using System.ComponentModel.DataAnnotations;

namespace TodoApplication.Api.Validation.Attributes;

public class DateTimeInFutureAttribute : ValidationAttribute
{ 
    private new string ErrorMessage => $"Date and time should be in future.";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return value switch
        {
            null => ValidationResult.Success,
            DateTime date when date < DateTime.Now => new ValidationResult(ErrorMessage),
            _ => ValidationResult.Success
        };
    }
}