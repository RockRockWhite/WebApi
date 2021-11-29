using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApi.DtoParameters;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {

        public readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository ?? throw new ArgumentException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{companyId}", Name = nameof(GetCompany))]
        public async Task<IActionResult> GetCompany(Guid companyId)
        {
            var company = await _companyRepository.GetCompanyAsync(companyId);
            return company != null ? Ok(_mapper.Map<CompanyDto>(company)) : NotFound();
        }

        [HttpGet(Name = nameof(GetCompanyies))]
        [HttpHead]
        public async Task<IActionResult> GetCompanyies([FromQuery] CompanyDtoParameters parameters)
        //public async Task<IActionResult> Companyies()
        {
            var companies = await _companyRepository.GetCompaniesAsync(parameters);

            var priviousPageLink = companies.HasProvious ? CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage) : null;
            var nextPageLink = companies.HasNext ? CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage) : null;

            var paginationMatadata = new
            {
                totalCount = companies.TotalCount,
                limit = companies.Limit,
                currentPage = companies.CurrentPage,
                totalPage = companies.TotalPages,
                priviousPageLink,
                nextPageLink
            };

            // 转义不安全字符
            // Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMatadata));
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMatadata, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            // return Ok(companyDtos);
            return Ok(companyDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany(CompanyAddDto company)
        {
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<CompanyDto>(entity);

            return CreatedAtRoute(nameof(GetCompany), new { companyId = returnDto.Id }, returnDto);
        }

        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var entity = await _companyRepository.GetCompanyAsync(companyId);

            if (entity is null)
            {
                return NotFound();
            }

            _companyRepository.DeleteCompany(entity);
            await _companyRepository.SaveAsync();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET,POST,OPTIONS");
            return Ok();
        }

        private string CreateCompaniesResourceUri(CompanyDtoParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link(nameof(GetCompanyies), new
                    {
                        offset = parameters.Offset - 1,
                        limit = parameters.Limit,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanyies), new
                    {
                        offset = parameters.Offset + 1,
                        limit = parameters.Limit,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                default:
                    return Url.Link(nameof(GetCompanyies), new
                    {
                        offset = parameters.Offset,
                        limit = parameters.Limit,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
            }
        }
    }
}