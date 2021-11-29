using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public abstract class EmployeeAddOrUpdateDtoBase : IValidatableObject
    {
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "{0}字段必填!!!")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度不能超过{1}")]
        public string FullName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FullName == "A")
            {
                yield return new ValidationResult("不能为A", new[] { nameof(FullName) });
            }
        }
    }
}
