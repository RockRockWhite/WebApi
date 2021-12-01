
namespace WebApi.Services
{
    public interface IPropertyMappingValue
    {
        IEnumerable<string> DestinationProperties { get; set; }
        bool Revert { get; set; }
    }
}