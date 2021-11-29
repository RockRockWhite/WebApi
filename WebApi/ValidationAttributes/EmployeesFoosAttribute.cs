using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.ValidationAttributes
{
    public class EmployeesFoosAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var addDto = validationContext.ObjectInstance as EmployeeAddOrUpdateDtoBase;
            if (addDto.FullName == "B")
            {
                return new ValidationResult(ErrorMessage, new[] { nameof(EmployeeAddOrUpdateDtoBase.FullName) });
            }

            return ValidationResult.Success;
        }
    }
}
