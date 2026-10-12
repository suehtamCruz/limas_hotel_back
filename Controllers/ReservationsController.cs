using LimasHotel.Api.Dtos;
using LimasHotel.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LimasHotel.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ReservationsController(IReservationService reservationService, IImageUploadService imageUploadService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationResponse>>> GetAll()
    {
        var reservations = await reservationService.GetAllAsync();
        return Ok(reservations.Select(ReservationResponse.From));
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationResponse>> GetById(string id)
    {
        var reservation = await reservationService.GetByIdAsync(id);
        return reservation is null ? NotFound() : Ok(ReservationResponse.From(reservation));
    }

    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> Create(CreateReservationRequest request)
    {
        if (request.EndDate < request.StartDate)
            return BadRequest(new { message = "'end_date' não pode ser anterior a 'start_date'." });

        var reservation = await reservationService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, ReservationResponse.From(reservation));
    }
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ReservationResponse>> Update(string id, UpdateReservationRequest request)
    {
        try
        {
            var reservation = await reservationService.UpdateAsync(id, request);
            return reservation is null ? NotFound() : Ok(ReservationResponse.From(reservation));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) =>
        await reservationService.DeleteAsync(id) ? NoContent() : NotFound();

    [HttpPost("{id}/dog-picture")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ReservationResponse>> UploadDogPicture(string id, IFormFile file)
    {
        if (await reservationService.GetByIdAsync(id) is null) return NotFound();
        if (file.Length == 0) return BadRequest(new { message = "Arquivo de imagem não informado." });

        await using var stream = file.OpenReadStream();
        var url = await imageUploadService.UploadAsync(stream, file.FileName);

        var reservation = await reservationService.SetDogPictureAsync(id, url);
        return reservation is null ? NotFound() : Ok(ReservationResponse.From(reservation));
    }

    [HttpPost("{id}/vaccine-card-picture")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ReservationResponse>> UploadVaccineCardPicture(string id, IFormFile file)
    {
        if (await reservationService.GetByIdAsync(id) is null) return NotFound();
        if (file.Length == 0) return BadRequest(new { message = "Arquivo de imagem não informado." });

        await using var stream = file.OpenReadStream();
        var url = await imageUploadService.UploadAsync(stream, file.FileName);

        var reservation = await reservationService.SetVaccineCardPictureAsync(id, url);
        return reservation is null ? NotFound() : Ok(ReservationResponse.From(reservation));
    }
}
