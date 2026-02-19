namespace API.Contracts.Types
{
    /// <summary>
    /// Enumerates the types of insurance claims.
    /// </summary>
    public enum ClaimType
    {
        Collision = 0,
        Grounding = 1,
        BadWeather = 2,
        Fire = 3
    }

    /// <summary>
    /// Enumerates the types of insurance covers.
    /// </summary>
    public enum CoverType
    {
        Yacht = 0,
        PassengerShip = 1,
        ContainerShip = 2,
        BulkCarrier = 3,
        Tanker = 4
    }
}
