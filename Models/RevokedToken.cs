using MongoDB.Bson.Serialization.Attributes;

namespace LimasHotel.Api.Models;

public class RevokedToken
{
    [BsonId]
    public string Jti { get; set; } = string.Empty;

    [BsonElement("expiresAt")]
    public DateTime ExpiresAt { get; set; }
}
