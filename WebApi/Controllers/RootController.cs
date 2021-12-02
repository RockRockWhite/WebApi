using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link(nameof(GetRoot), new { }), "self", "GET"));

            links.Add(new LinkDto(Url.Link(nameof(CompaniesController.GetCompanyies), new { }), "companies", "GET"));

            links.Add(new LinkDto(Url.Link(nameof(CompaniesController.CreateCompany), new { }), "create_companie", "POST"));

            return Ok(links);
        }
    }
}
