namespace WebApi.DtoParameters
{
    public class CompanyDtoParameters
    {
        private const int MaxPageSize = 20;
        public string? CompanyName { get; set; }
        public string? SearchTerm { get; set; }
        public int Offset { get; set; } = 0;

        private int _limit = 20;

        public int Limit
        {
            get => _limit;
            set => _limit = (_limit > MaxPageSize) ? MaxPageSize : value;
        }
        public string? OrderBy { get; set; } = "Id";
        public string? Fields { get; set; }
    }

}
