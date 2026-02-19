namespace API.Contracts.Requests
{
    /// <summary>
    /// Request model for computing premium.
    /// </summary>
    public class ComputePremiumRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public API.Contracts.Types.CoverType Type { get; set; }
    }
}
