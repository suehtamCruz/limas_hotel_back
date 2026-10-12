namespace LimasHotel.Api.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "LimasHotel";
    public string Audience { get; set; } = "LimasHotel";
    public string Key { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
