namespace API.Contracts.Requests
{
    /// <summary>
    /// Request model for creating a new claim.
    /// </summary>
    public class CreateClaimRequest
    {
        public string CoverId { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public API.Contracts.Types.ClaimType Type { get; set; }
        public decimal DamageCost { get; set; }
    }
}
