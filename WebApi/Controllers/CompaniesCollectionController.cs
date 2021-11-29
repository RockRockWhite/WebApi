using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesCollectionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICompanyRepository _companyRepository;

        public CompaniesCollectionController(IMapper mapper, ICompanyRepository companyRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompaniesCollection(IEnumerable<CompanyAddDto> companiesColletcion)
        {
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companiesColletcion);

            foreach (var companyEntity in companyEntities)
            {
                _companyRepository.AddCompany(companyEntity);
            }

            await _companyRepository.SaveAsync();

            var returnDtos = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);

            var companyIds = string.Join(",", returnDtos.Select(x => x.Id));

            return CreatedAtRoute(nameof(GetCompaniesCollection), new { companyIds = companyIds }, returnDtos);
        }

        [HttpGet("{companyIds}", Name = nameof(GetCompaniesCollection))]
        public async Task<IActionResult> GetCompaniesCollection(
            [FromRoute]
            [ModelBinder(BinderType = typeof(ArrayModelBinder))]
            IEnumerable<Guid> companyIds)
        {
            if (companyIds is null)
            {
                return BadRequest();
            }

            var entities = await _companyRepository.GetCompaniesAsync(companyIds);

            if (entities.Count() != companyIds.Count())
            {
                return NotFound();
            }

            var returnDtos = _mapper.Map<IEnumerable<CompanyDto>>(entities);

            return Ok(returnDtos);
        }
    }
}
