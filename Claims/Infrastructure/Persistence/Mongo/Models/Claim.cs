using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.Persistence.Mongo.Models
{
    public class Claim
    {
        [BsonId]
        public string Id { get; set; } = string.Empty;

        [BsonElement("coverId")]
        public string CoverId { get; set; } = string.Empty;

        [BsonElement("created")]
        public DateTime Created { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("claimType")]
        public ClaimType Type { get; set; }

        [BsonElement("damageCost")]
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