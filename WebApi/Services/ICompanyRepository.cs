using WebApi.DtoParameters;
using WebApi.Entities;

namespace WebApi.Services
{
    public interface ICompanyRepository
    {
        void AddCompany(Company company);
        Task<Company> GetCompanyAsync(Guid companyId);
        Task<IEnumerable<Company>> GetCompaniesAsync(CompanyDtoParameters parameters);
        Task<IEnumerable<Company>> GetCompaniesAsync(IEnumerable<Guid> companyIds);

        void UpdateCompany(Company company);
        void DeleteCompany(Company company);
        Task<bool> CompanyExistsAsync(Guid companyId);
        public Task<bool> SaveAsync();
    }
}
