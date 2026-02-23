namespace Domain.Entities
{
    public class Claim
    {
        public Claim(string coverId, DateTime created, string name, ClaimType type, decimal damageCost)
        {
            Id = Guid.NewGuid().ToString();
            CoverId = coverId;
            Created = created;
            Name = name;
            Type = type;
            DamageCost = damageCost;
        }

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
