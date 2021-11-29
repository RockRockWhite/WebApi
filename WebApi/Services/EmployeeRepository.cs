using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities;

namespace WebApi.Services
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly RoutineDbContext _context;

        public EmployeeRepository(RoutineDbContext dbContext)
        {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, String? name, String? q)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            var items = _context.Employees.Where(x => x.CompanyId == companyId);

            if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(q))
            {
                return await items.OrderBy(x => x.Id).ToListAsync();
            }

            if (!String.IsNullOrEmpty(name))
            {
                name = name?.Trim();
                items = items.Where(x => x.Name == name);
            }

            if (!String.IsNullOrEmpty(q))
            {
                q = q?.Trim();
                items = items.Where(x => x.Name.Contains(q));
            }

            return await items.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }
            if (employeeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(employeeId));
            }
            return await _context.Employees.FirstOrDefaultAsync(x => x.Id == employeeId && x.CompanyId == companyId);
        }

        public void AddEmployee(Guid companyId, Employee employee)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(companyId));
            }

            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee));
            }

            employee.Id = Guid.NewGuid();
            employee.CompanyId = companyId;

            _context.Employees.Add(employee);
        }

        // 本方法与存储技术无关
        public void UpdateEmployee(Employee employee)
        {
            // _context.Entry(employee).State = EntityState.Modified;
        }
    }
}
