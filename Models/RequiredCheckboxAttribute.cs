using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyContact.Models;


public class RequiredCheckboxAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        System.Console.WriteLine(value);
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult("Please select at least one group.");
        }
        return ValidationResult.Success;
    }
}
