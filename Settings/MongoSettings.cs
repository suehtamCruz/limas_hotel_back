namespace LimasHotel.Api.Settings;

public class MongoSettings
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "limas_hotel";
    public string UsersCollection { get; set; } = "users";
    public string RevokedTokensCollection { get; set; } = "revoked_tokens";
}
