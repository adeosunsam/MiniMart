using System.ComponentModel.DataAnnotations;

namespace MiniMart.Infrastructure.Attributes
{
    public class GreaterThanZeroAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var valueObject = value as int? ?? value as long? ?? value as decimal?;
            if (valueObject.HasValue && valueObject > 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"{validationContext.DisplayName} must be greater than 0.");
        }
    }
}
