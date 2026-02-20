namespace Domain.Entities
{
    public class Claim
    {
        public string Id { get; set; } = string.Empty;

        public string CoverId { get; set; } = string.Empty;
        public DateTime Created { get; set; }

        public string Name { get; set; } = string.Empty;

        public ClaimType Type { get; set; }

        public decimal DamageCost { get; set; }
    }
    public enum ClaimType
    {
        Collision = 0,
        Grounding = 1,
        BadWeather = 2,
        Fire = 3
    }
}
