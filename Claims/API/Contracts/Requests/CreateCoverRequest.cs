namespace API.Contracts.Requests
{
    /// <summary>
    /// Request model for creating a new cover.
    /// </summary>
    public class CreateCoverRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public API.Contracts.Types.CoverType Type { get; set; }
        public decimal Premium { get; set; }
    }
}
