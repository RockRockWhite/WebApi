namespace WebApi.Services
{
    public class PropertyMappingValue
    {
        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationProperties = destinationProperties ?? throw new ArgumentNullException(nameof(DestinationProperties));
            Revert = revert;
        }

        public IEnumerable<string> DestinationProperties { get; set; }

        public bool Revert { get; set; }
    }
}
