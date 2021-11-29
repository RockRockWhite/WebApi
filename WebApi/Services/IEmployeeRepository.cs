using WebApi.Data;
using WebApi.Entities;

namespace WebApi.Services
{
    public interface IEmployeeRepository
    {
        public Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, String? name, String? q);
        public Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId);
        public void AddEmployee(Guid companyId, Employee employee);
        public void UpdateEmployee(Employee employee);
        public void DeleteEmployee(Employee employee);
    }
}
