namespace API.Contracts.Responses
{
    /// <summary>
    /// Response model for returning cover data.
    /// </summary>
    public class CoverResponse
    {
        public string Id { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public API.Contracts.Types.CoverType Type { get; set; }
        public decimal Premium { get; set; }
    }
}
