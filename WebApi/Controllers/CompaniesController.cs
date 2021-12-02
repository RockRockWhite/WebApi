using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper, IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyCheckerService)
        {
            _companyRepository = companyRepository ?? throw new ArgumentException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService;
        }

        [Produces("application/json",
            "application/vnd.rock.hateoas+json",
            "application/vnd.rock.company.full.hateoas+json",
            "application/vnd.rock.company.friendly.hateoas+json",
            "application/vnd.rock.company.full+json",
            "application/vnd.rock.company.friendly+json")]
        [HttpGet("{companyId}", Name = nameof(GetCompany))]
        public async Task<IActionResult> GetCompany(Guid companyId, string? fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            // TODO: 本来这里应该判断fields是否合法

            if (!MediaTypeHeaderValue.TryParse(mediaType, out var mediaTypeValue))
            {
                return BadRequest();
            }

            var company = await _companyRepository.GetCompanyAsync(companyId);

            var shapedData = _mapper.Map<CompanyDto>(company).ShapeData(fields);


            var includeLinks = mediaTypeValue.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
            if (includeLinks)
            {
                shapedData.TryAdd("links", CreateLinksForCompany(companyId, fields));
            }

            // 删除hateoas后缀
            var primaryMediaType = mediaTypeValue.SubTypeWithoutSuffix.ToString().Replace(".hateoas", "", StringComparison.InvariantCultureIgnoreCase);

            if (primaryMediaType == "vnd.rock.company.full")
            {
                shapedData.TryAdd("FullDto", "FOOBAR");
            }

            if (primaryMediaType == "vnd.rock.company.friendly")
            {
                shapedData.TryAdd("FriendlyDto", "FOOBAR");
            }

            return company != null ? Ok(shapedData) : NotFound();
        }

        [HttpGet(Name = nameof(GetCompanyies))]
        [HttpHead]
        public async Task<IActionResult> GetCompanyies([FromQuery] CompanyDtoParameters parameters)
        //public async Task<IActionResult> Companyies()
        {
            if (!_propertyMappingService.ValidMappingExists<CompanyDto, Company>(parameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.TypeHasProperties<CompanyDto>(parameters.Fields))
            {
                return BadRequest();
            }

            var companies = await _companyRepository.GetCompaniesAsync(parameters);

            var paginationMatadata = new
            {
                totalCount = companies.TotalCount,
                limit = companies.Limit,
                currentPage = companies.CurrentPage,
                totalPage = companies.TotalPages,
            };

            // 转义不安全字符
            // Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMatadata));
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMatadata, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            var shapedData = companyDtos.ShapeData(parameters.Fields);

            var companiesWhiteLinks = shapedData.Select(c =>
            {
                var companyDict = c as IDictionary<string, object>;
                companyDict.Add("Links", CreateLinksForCompany((Guid)companyDict["Id"], null));

                return companyDict;
            });

            var linkedCollectionResource = new
            {
                Values = companiesWhiteLinks,
                Links = CreateLinksForCompany(parameters, companies.HasProvious, companies.HasNext),
            };

            // return Ok(companyDtos);
            return Ok(linkedCollectionResource);
        }

        [HttpPost(Name = nameof(CreateCompany))]
        public async Task<IActionResult> CreateCompany(CompanyAddDto company)
        {
            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<CompanyDto>(entity);
            var shapeDto = returnDto.ShapeData(null);
            shapeDto.TryAdd("links", CreateLinksForCompany(returnDto.Id, null));

            return CreatedAtRoute(nameof(GetCompany), new { companyId = returnDto.Id }, shapeDto);
        }

        [HttpDelete("{companyId}", Name = nameof(DeleteCompany))]
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
                        fields = parameters.Fields,
                        offset = parameters.Offset - 1,
                        limit = parameters.Limit,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                case ResourceUriType.NextPage:
                    return Url.Link(nameof(GetCompanyies), new
                    {
                        fields = parameters.Fields,
                        offset = parameters.Offset + 1,
                        limit = parameters.Limit,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
                default:
                    return Url.Link(nameof(GetCompanyies), new
                    {
                        fields = parameters.Fields,
                        offset = parameters.Offset,
                        limit = parameters.Limit,
                        companyName = parameters.CompanyName,
                        searchTerm = parameters.SearchTerm
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(Guid companyId, string fields)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link(nameof(GetCompany), new { companyId, fields }), "self", "GET"));
            links.Add(new LinkDto(Url.Link(nameof(DeleteCompany), new { companyId }), "delete_company", "DELETE"));

            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.CreateEmployee), new { companyId }), "create_employee_for_company", "POST"));
            links.Add(new LinkDto(Url.Link(nameof(EmployeesController.GetEmployees), new { companyId }), "get_employees_for_company", "GET"));

            return links;
        }

        private IEnumerable<LinkDto> CreateLinksForCompany(CompanyDtoParameters parameters, bool hasPrivious, bool hasNext)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.CurrentPage), "self", "GET"));

            if (hasPrivious)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.PreviousPage), "privious_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(new LinkDto(CreateCompaniesResourceUri(parameters, ResourceUriType.NextPage), "next_page", "GET"));
            }

            return links;
        }
    }
}