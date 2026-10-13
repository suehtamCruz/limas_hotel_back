using LimasHotel.Api.Data;
using LimasHotel.Api.Dtos;
using LimasHotel.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LimasHotel.Api.Services;

public class ReservationService(MongoContext context) : IReservationService
{
    private const decimal AirConditionerDailyRate = 100m;
    private const decimal StandardDailyRate = 70m;

    private readonly IMongoCollection<Reservation> _reservations = context.Reservations;

    public Task<List<Reservation>> GetAllAsync() =>
        _reservations.Find(_ => true).SortBy(r => r.StartDate).ToListAsync();

    public async Task<Reservation?> GetByIdAsync(string id) =>
        ObjectId.TryParse(id, out _)
            ? await _reservations.Find(r => r.Id == id).FirstOrDefaultAsync()
            : null;

    public async Task<Reservation> CreateAsync(CreateReservationRequest request, string? dogPictureUrl, string? vaccineCardPictureUrl)
    {
        var reservation = new Reservation
        {
            Owner = request.Owner.Trim(),
            DogName = request.DogName.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DogBreed = request.DogBreed.Trim(),
            DogAge = request.DogAge,
            Phone = request.Phone.Trim(),
            AirConditioner = request.AirConditioner
        }; 
        if (dogPictureUrl is not null)
        {
            reservation.DogPicture = dogPictureUrl;
        }

        if (vaccineCardPictureUrl is not null)
        {
            reservation.VaccineCardPicture = vaccineCardPictureUrl;
        }

        reservation.Price = CalculatePrice(reservation.StartDate, reservation.EndDate, reservation.AirConditioner);

        await _reservations.InsertOneAsync(reservation);
        return reservation;
    }

    public async Task<Reservation> CreateAsync(CreateReservationRequest request, string? dogPictureUrl)
    {
        var reservation = new Reservation
        {
            Owner = request.Owner.Trim(),
            DogName = request.DogName.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            DogBreed = request.DogBreed.Trim(),
            DogAge = request.DogAge,
            Phone = request.Phone.Trim(),
            AirConditioner = request.AirConditioner
        };
        if (dogPictureUrl is not null)
        {
            reservation.DogPicture = dogPictureUrl;
        }

        reservation.Price = CalculatePrice(reservation.StartDate, reservation.EndDate, reservation.AirConditioner);

        await _reservations.InsertOneAsync(reservation);
        return reservation;
    }

    public async Task<Reservation?> UpdateAsync(string id, UpdateReservationRequest request)
    {
        var reservation = await GetByIdAsync(id);
        if (reservation is null) return null;

        if (request.Owner is not null) reservation.Owner = request.Owner.Trim();
        if (request.DogName is not null) reservation.DogName = request.DogName.Trim();
        if (request.StartDate is not null) reservation.StartDate = request.StartDate.Value;
        if (request.EndDate is not null) reservation.EndDate = request.EndDate.Value;
        if (request.DogBreed is not null) reservation.DogBreed = request.DogBreed.Trim();
        if (request.DogAge is not null) reservation.DogAge = request.DogAge.Value;
        if (request.Phone is not null) reservation.Phone = request.Phone.Trim();
        if (request.AirConditioner is not null) reservation.AirConditioner = request.AirConditioner.Value;

        if (reservation.EndDate < reservation.StartDate)
            throw new ArgumentException("'end_date' não pode ser anterior a 'start_date'.");

        reservation.Price = CalculatePrice(reservation.StartDate, reservation.EndDate, reservation.AirConditioner);
        reservation.UpdatedAt = DateTime.UtcNow;

        await _reservations.ReplaceOneAsync(r => r.Id == id, reservation);
        return reservation;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return false;
        var result = await _reservations.DeleteOneAsync(r => r.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<Reservation?> SetDogPictureAsync(string id, string pictureUrl)
    {
        var reservation = await GetByIdAsync(id);
        if (reservation is null) return null;

        reservation.DogPicture = pictureUrl;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _reservations.ReplaceOneAsync(r => r.Id == id, reservation);
        return reservation;
    }

    public async Task<Reservation?> SetVaccineCardPictureAsync(string id, string pictureUrl)
    {
        var reservation = await GetByIdAsync(id);
        if (reservation is null) return null;

        reservation.VaccineCardPicture = pictureUrl;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _reservations.ReplaceOneAsync(r => r.Id == id, reservation);
        return reservation;
    }

    private static decimal CalculatePrice(DateTime startDate, DateTime endDate, bool airConditioner)
    {
        var dailyRate = airConditioner ? AirConditionerDailyRate : StandardDailyRate;
        var dailyStays = (endDate.Date - startDate.Date).Days + 1;
        return dailyStays * dailyRate;
    }
}
