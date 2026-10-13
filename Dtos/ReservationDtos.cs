using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LimasHotel.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace LimasHotel.Api.Dtos;

public record CreateReservationRequest(
    [Required, MinLength(2)] string Owner,
    [Required, MinLength(1)] [property: JsonPropertyName("dog_name")] [property: FromForm(Name = "dog_name")] string DogName,
    [Required] [property: JsonPropertyName("start_date")] [property: FromForm(Name = "start_date")] DateTime StartDate,
    [Required] [property: JsonPropertyName("end_date")] [property: FromForm(Name = "end_date")] DateTime EndDate,
    [Required, MinLength(1)] [property: JsonPropertyName("dog_breed")] [property: FromForm(Name = "dog_breed")] string DogBreed,
    [Range(0, 50)] [property: JsonPropertyName("dog_age")] [property: FromForm(Name = "dog_age")] int DogAge,
    [Required, Phone] string Phone,
    [property: JsonPropertyName("airConditioner")] [property: FromForm(Name = "airConditioner")] bool AirConditioner,
    [Required] [property: FromForm(Name = "dog_picture")] IFormFile? DogPicture,
    [Required] [property: FromForm(Name = "vaccine_card_picture")] IFormFile? VaccineCardPicture);

public record UpdateReservationRequest(
    [MinLength(2)] string? Owner,
    [MinLength(1)] [property: JsonPropertyName("dog_name")] string? DogName,
    [property: JsonPropertyName("start_date")] DateTime? StartDate,
    [property: JsonPropertyName("end_date")] DateTime? EndDate,
    [MinLength(1)] [property: JsonPropertyName("dog_breed")] string? DogBreed,
    [Range(0, 50)] [property: JsonPropertyName("dog_age")] int? DogAge,
    [Phone] string? Phone,
    [property: JsonPropertyName("airConditioner")] bool? AirConditioner);

public record ReservationResponse(
    string Id,
    string Owner,
    [property: JsonPropertyName("dog_name")] string DogName,
    [property: JsonPropertyName("start_date")] DateTime StartDate,
    [property: JsonPropertyName("end_date")] DateTime EndDate,
    [property: JsonPropertyName("dog_breed")] string DogBreed,
    [property: JsonPropertyName("dog_age")] int DogAge,
    string Phone,
    [property: JsonPropertyName("dog_picture")] string? DogPicture,
    [property: JsonPropertyName("vaccine_card_picture")] string? VaccineCardPicture,
    [property: JsonPropertyName("airConditioner")] bool AirConditioner,
    decimal Price,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static ReservationResponse From(Reservation reservation) => new(
        reservation.Id,
        reservation.Owner,
        reservation.DogName,
        reservation.StartDate,
        reservation.EndDate,
        reservation.DogBreed,
        reservation.DogAge,
        reservation.Phone,
        reservation.DogPicture,
        reservation.VaccineCardPicture,
        reservation.AirConditioner,
        reservation.Price,
        reservation.CreatedAt,
        reservation.UpdatedAt);
}
