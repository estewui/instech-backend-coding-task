namespace API.Contracts.Responses
{
    /// <summary>
    /// Response model for returning claim data.
    /// </summary>
    public class ClaimResponse
    {
        public string Id { get; set; } = string.Empty;
        public string CoverId { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public string Name { get; set; } = string.Empty;
        public API.Contracts.Types.ClaimType Type { get; set; }
        public decimal DamageCost { get; set; }
    }
}
