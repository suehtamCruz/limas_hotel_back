using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LimasHotel.Api.Models;

public class Reservation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("owner")]
    public string Owner { get; set; } = string.Empty;

    [BsonElement("dog_name")]
    public string DogName { get; set; } = string.Empty;

    [BsonElement("start_date")]
    public DateTime StartDate { get; set; }

    [BsonElement("end_date")]
    public DateTime EndDate { get; set; }

    [BsonElement("dog_breed")]
    public string DogBreed { get; set; } = string.Empty;

    [BsonElement("dog_age")]
    public int DogAge { get; set; }

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
