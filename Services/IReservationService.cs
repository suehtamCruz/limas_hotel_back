using LimasHotel.Api.Dtos;
using LimasHotel.Api.Models;

namespace LimasHotel.Api.Services;

public interface IReservationService
{
    Task<List<Reservation>> GetAllAsync();
    Task<Reservation?> GetByIdAsync(string id);
    Task<Reservation> CreateAsync(CreateReservationRequest request);
    Task<Reservation?> UpdateAsync(string id, UpdateReservationRequest request);
    Task<bool> DeleteAsync(string id);
    Task<Reservation?> SetDogPictureAsync(string id, string pictureUrl);
    Task<Reservation?> SetVaccineCardPictureAsync(string id, string pictureUrl);
}
