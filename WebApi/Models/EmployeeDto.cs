using System;

namespace WebApi.Models
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
    }
}
