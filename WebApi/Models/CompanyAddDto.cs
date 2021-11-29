using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

namespace WebApi.Models
{
    public class CompanyAddDto
    {
        [Display(Name = "公司名")]
        [Required(ErrorMessage = "{0}字段必填!!!")]
        [MaxLength(100, ErrorMessage = "{0}的最大长度不能超过{1}")]
        public string Name { get; set; }

        [Display(Name = "简介")]
        [Required(ErrorMessage = "{0}字段必填!!!")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "{0}的最大长度需要在{2}-{1}")]
        public string Introduction { get; set; }

        public ICollection<EmployeeAddDto> Employees { get; set; } = new List<EmployeeAddDto>();
    }
}
