using LimasHotel.Api.Models;
using LimasHotel.Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LimasHotel.Api.Data;

public class MongoContext
{
    public IMongoCollection<User> Users { get; }
    public IMongoCollection<RevokedToken> RevokedTokens { get; }
    public IMongoCollection<Reservation> Reservations { get; }

    public MongoContext(IOptions<MongoSettings> options)
    {
        var settings = options.Value;
        var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);

        Users = database.GetCollection<User>(settings.UsersCollection);
        RevokedTokens = database.GetCollection<RevokedToken>(settings.RevokedTokensCollection);
        Reservations = database.GetCollection<Reservation>(settings.ReservationsCollection);

        EnsureIndexes();
    }

    private void EnsureIndexes()
    {
        Users.Indexes.CreateOne(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true }));

        // TTL index: Mongo remove automaticamente tokens revogados após expirarem
        RevokedTokens.Indexes.CreateOne(new CreateIndexModel<RevokedToken>(
            Builders<RevokedToken>.IndexKeys.Ascending(t => t.ExpiresAt),
            new CreateIndexOptions { ExpireAfter = TimeSpan.Zero }));
    }
}
