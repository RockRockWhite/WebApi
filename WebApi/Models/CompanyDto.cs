using System;

namespace WebApi.Models
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public DateTime? BankruptTime { get; set; }
    }
}
