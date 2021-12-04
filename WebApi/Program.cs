using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using WebApi.Data;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 添加缓存器
builder.Services.AddResponseCaching();

builder.Services.AddControllers(option =>
{
    option.ReturnHttpNotAcceptable = true;

    option.CacheProfiles.Add("120sCacheProFile", new CacheProfile { Duration = 120 });

    //option.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
    //option.OutputFormatters.Insert(0, new XmlDataContractSerializerOutputFormatter());
})
.ConfigureApiBehaviorOptions(setup =>
{
    setup.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://rockrockwhite.cn",
            Title = "有错误!!!",
            Status = StatusCodes.Status422UnprocessableEntity,
            Instance = context.HttpContext.Request.Path
        };

        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

        return new UnprocessableEntityObjectResult(problemDetails) { ContentTypes = { "application/problem+json" } };
    };

}).AddNewtonsoftJson();

builder.Services.Configure<MvcOptions>(config =>
{
    var newtonSoftJsonOutPutFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

    newtonSoftJsonOutPutFormatter?.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
});

// 添加ETag支持
builder.Services.AddHttpCacheHeaders(expires =>
{
    expires.MaxAge = 60;
    expires.CacheLocation = CacheLocation.Private;
}, validation =>
{
    validation.MustRevalidate = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddDbContext<RoutineDbContext>(option =>
{
    option.UseMySql("Server=localhost;Database=routine;User=root;Password=white;", new MySqlServerVersion(new Version(8, 0, 26)));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IPropertyMappingService, PropertyMappingService>();
builder.Services.AddSingleton<IPropertyCheckerService, PropertyCheckerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Unexpected Error");
        });
    });
}

//// 使用缓存器 不支持Etag
//app.UseResponseCaching();

// 使用Etag
app.UseHttpCacheHeaders();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<RoutineDbContext>();

    // context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();